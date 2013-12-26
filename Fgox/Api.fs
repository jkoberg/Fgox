namespace Fgox

open FSharp.Data.Json
open FSharp.Data.Json.Extensions


type Api(secrets:Fgox.Config.Secrets) =
  let client = new System.Net.Http.HttpClient()
  
  interface System.IDisposable with
    member x.Dispose() =
      client.Dispose()

  member this.sell (qty:decimal) (price:decimal) = async {
    let req =
      Fgox.Http.makePost secrets "api/1/private/order/add" [
        "type", "ask" 
        "amount_int", (qty * 100000000M |> round).ToString()
        "price_int", (price * 100000M |> round).ToString()
        ]
    let! resp = req|> client.SendAsync |> Async.AwaitTask
    let! body = resp.Content.ReadAsStringAsync() |> Async.AwaitTask
    return body
    }

  member this.getOrders = async {
    let req =  Fgox.Http.makePost secrets "api/1/generic/private/orders" []
    let! resp = req|> client.SendAsync |> Async.AwaitTask
    let! body = resp.Content.ReadAsStringAsync() |> Async.AwaitTask
    return
      match JsonValue.Parse body with
      | JsonValue.Array items ->
        [for item in items -> (item?oid.AsString(), item?``type``.AsString())]
      | _ -> failwith "Didn't find json array in body of orders response" 
    }
