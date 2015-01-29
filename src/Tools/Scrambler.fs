namespace Tools

open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Security.Cryptography
open System.Windows.Forms

[<AbstractClass; Sealed>]
type Scrambler private () = 
    
    [<Literal>]
    static let e_csum = 0x12
    
    [<Literal>]
    static let table = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
    
    static let randomizer = new Random()
    
    static let createString min max = 
        randomizer.Next(min, max)
        |> Array.zeroCreate
        |> Array.map (fun _ -> table.[randomizer.Next(table.Length)])
        |> fun array -> new string(array)
    
    static let createCSum size = 
        let csum = Array.zeroCreate size
        use rngCsp = new RNGCryptoServiceProvider()
        rngCsp.GetBytes csum
        csum
    
    static let encryptFile srcPath = 
        let fileData = File.ReadAllBytes srcPath
        let csum = createCSum 4
        Buffer.BlockCopy(csum, 0, fileData, e_csum, csum.Length)
        let dstFileName = createString 3 12
        let dstPath = Path.Combine(Path.GetDirectoryName srcPath, dstFileName + ".exe")
        File.WriteAllBytes(dstPath, fileData)
        dstPath
    
    static member Scramble parameter = 
        let showError message = MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
        match parameter with
        | p when String.IsNullOrWhiteSpace(p) -> 
            let mutable result = true
            try 
                let srcPath = Assembly.GetEntryAssembly().Location
                if Path.GetFileName(srcPath).Equals("Calculator.exe", StringComparison.InvariantCultureIgnoreCase) 
                   || (new FileInfo(srcPath)).CreationTimeUtc < DateTime.UtcNow.AddDays -1.0 then 
                    let dstPath = encryptFile srcPath
                    Process.Start(dstPath, sprintf "\"%s\"" srcPath) |> ignore
                else result <- false
            with ex -> showError (sprintf "Failed to encrypt a file: %s" ex.Message)
            result
        | oldFile -> 
            try 
                File.Delete oldFile
            with _ -> 
                let filename = Path.GetFileName oldFile
                showError (sprintf "Failed to delete \"%s\" file" filename)
            false
