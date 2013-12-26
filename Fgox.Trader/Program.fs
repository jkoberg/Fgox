module Fgox.Main

open Fgox.Config

module Synchronous =
  open Nito.AsyncEx.Synchronous
  let ofAsync a = (Async.StartAsTask a).WaitAndUnwrapException()

[<EntryPoint>]
let main argv =
  Synchronous.ofAsync <| async {
    let! secrets = Secrets.fromFile "../../fgox_secrets.json"
    use gox = new Fgox.Api(secrets)
    do! Fgox.Trader.Run gox
    printfn "Press any key"
    ignore <| System.Console.ReadKey()
    return 0
  }
    

  