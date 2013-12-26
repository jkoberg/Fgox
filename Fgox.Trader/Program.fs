module Fgox.Main

open Fgox.Config

module Synchronous =
  open Nito.AsyncEx.Synchronous
  let ofAsync a = (Async.StartAsTask a).WaitAndUnwrapException()

[<EntryPoint>]
let main argv =
  printfn "%s" (System.IO.Directory.GetCurrentDirectory())
  Synchronous.ofAsync <| async {
    let! secrets = Secrets.fromFile "../../fgox_secrets.json"
    use api = new Fgox.Api(secrets)
    do! Fgox.Trader.execTrades api
    return 0
  }
    

  