module CurriculumParser.Tests

open NUnit.Framework
open FsUnitTyped

open CurriculumParser

let mutable curriculum = Curriculum("resources/20_5162_1.docx")

[<SetUp>]
let Setup () =
    curriculum <- Curriculum("resources/20_5162_1.docx")

[<Test>]
let ``All 28 competences shall be parsed`` () =
    curriculum.Competences |> shouldHaveLength 28

[<Test>]
let ``Competences shall contain УКБ-1`` () =
    let competenceUnderTest = curriculum.Competences |> Seq.filter (fun c -> c.Code = "УКБ-1") |> Seq.head
    competenceUnderTest.Code |> shouldEqual "УКБ-1"
    competenceUnderTest.Description 
        |> shouldEqual "Способен участвовать в разработке и реализации проектов, в т.ч. предпринимательских"

[<Test>]
let ``Disciplines shall contain four instances of Mathematical Analysis`` () =
    let disciplinesUnderTest = curriculum.Disciplines |> Seq.filter (fun d -> d.RegNumber = "002182")
    disciplinesUnderTest |> shouldHaveLength 4

    disciplinesUnderTest
    |> Seq.iter(fun d ->
                    d.Name |> shouldEqual "Математический анализ"
                    d.EnglishName |> shouldEqual "Mathematical Analysis")

[<Test>]
let ``Physical Training is parsed properly`` () =
    let disciplinesUnderTest = curriculum.Disciplines |> Seq.filter (fun d -> d.RegNumber = "900000")

    disciplinesUnderTest
    |> Seq.exists(fun d ->
                    d.Name = "Физическая культура и спорт (прог эл обуч), осн тр"
                    && d.EnglishName = "Physical Training and Sport")
    |> shouldEqual true

[<Test>]
let ``Last discipline is parsed properly`` () =
    let disciplineUnderTest = curriculum.Disciplines |> Seq.filter (fun d -> d.RegNumber = "038167") |> Seq.exactlyOne

    disciplineUnderTest.Name |> shouldEqual "Стандарты параллельного программирования"
    disciplineUnderTest.EnglishName |> shouldEqual "Parallel Programming Standards"
