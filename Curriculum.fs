namespace CurriculumParser

open DocumentFormat.OpenXml.Packaging
open DocumentFormat.OpenXml.Wordprocessing
open System.Linq
open System.Text.RegularExpressions

/// Information about competence formed as a result of studying.
type Competence(code: string, description: string) =
    /// Competence code, as in curriculum (ОПК-1, for example).
    member this.Code = code

    /// Competence description, as in curriculum.
    member this.Description = description

/// Information about a discipline.
type Discipline(regNumber: string, name: string, englishName: string) =
    /// Discipline registration number, for example 002182 for Mathematical Analysis.
    member this.RegNumber = regNumber

    /// Discipline russian name, for example Математический анализ.
    member this.Name = name

    /// Discipline english name, for example Mathematical Analysis.
    member this.EnglishName = englishName

/// Parses curriculum Word document and provides 
type Curriculum (fileName: string) =
    let mutable competences = []
    let mutable disciplines = []

    let parseCompetences (body: Body) =
        let competenceTable = body.Elements<Table>().Skip(2).First()

        for row in competenceTable.Elements<TableRow>().Skip(1) do
            let competenceCode = row.Elements<TableCell>().ElementAt(0).InnerText
            let competenceDescription = row.Elements<TableCell>().ElementAt(1).InnerText
            competences <- Competence(competenceCode, competenceDescription) :: competences

    let parseDiscipline (row: TableRow) =
        if row.Elements<TableCell>().Count() = 20 then
            let nameCell = row.Elements<TableCell>().ElementAt(3)
            let runs = nameCell.Descendants<Run>()
            let mutable name = ""
            let mutable englishName = ""
            let mutable seenBreak = false
            for run in runs do
                for element in run.ChildElements do
                    match element with
                    | :? Text when not seenBreak-> name <- name + element.InnerText
                    | :? Text when seenBreak-> englishName <- englishName + element.InnerText
                    | :? Break -> seenBreak <- true
                    | _ -> ()
            let regexMatch = Regex.Match(name, @"\[(\d+)\]\s+(.+)")
            if regexMatch.Success then 
                let regNumber = regexMatch.Groups.[1].Value
                let russianName = regexMatch.Groups.[2].Value
                disciplines <- Discipline(regNumber, russianName, englishName) :: disciplines

    let parseDisciplines (body: Body) =
        let mainTable = body.Elements<Table>().Skip(3).First()

        for row in mainTable.Elements<TableRow>().Skip(2) do
           parseDiscipline row

        disciplines <- List.rev disciplines

    do 
        use wordDocument = WordprocessingDocument.Open(fileName, false)

        let body = wordDocument.MainDocumentPart.Document.Body
        parseCompetences body
        parseDisciplines body

    /// A list of all competences in a curriculum.
    member this.Competences: Competence seq = List.toSeq competences

    /// A list of all disciplines in a curriculum.
    member this.Disciplines: Discipline seq = List.toSeq disciplines
