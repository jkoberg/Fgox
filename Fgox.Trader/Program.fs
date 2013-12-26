module Fgox.Main

open Nito.AsyncEx.Synchronous
open Fgox.Config

[<EntryPoint>]
let main argv =
  (Async.StartAsTask <| async {
    let! secrets = Secrets.fromFile "../../fgox_secrets.json"
    do! new Fgox.Api(secrets) |> Fgox.Trader.Run
    return 0
  }).WaitAndUnwrapException()
    

  