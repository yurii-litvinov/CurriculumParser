open CurriculumParser
open System.IO

let usage () =
    printfn "Curriculum parser."
    printfn "Gets a list of curriculum dusciplines from a given Word curriculum file and prints them to a text file."
    printfn "Usage: dotnet run -- <file name>"
    printfn "<filename.txt> will contain output."

[<EntryPoint>]
let main argv =
    if argv.Length <> 1 then
        usage ()
    else
        let curriculum = Curriculum(argv.[0])

        let outputFileName = Path.GetFileNameWithoutExtension argv.[0]

        use outputFile = new StreamWriter(outputFileName + ".txt")

        for discipline in curriculum.Disciplines do
            async {
                let formattedString = sprintf "[%s] %s" discipline.RegNumber discipline.Name
                do! outputFile.WriteLineAsync formattedString |>  Async.AwaitTask
            } |> Async.RunSynchronously

        ()
    0 