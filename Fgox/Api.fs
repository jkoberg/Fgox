namespace Fgox

open FSharp.Data.Json
open FSharp.Data.Json.Extensions

type Api(secrets:Fgox.Config.Secrets) =
  let client = new System.Net.Http.HttpClient()
  
  let send pathsuffix payload = 
    Fgox.Http.makePost secrets pathsuffix payload |> client.SendAsync |> Async.AwaitTask

  interface System.IDisposable with
    member x.Dispose() =
      client.Dispose()

  member this.sell (qty:decimal) (price:decimal) = async {
    let! resp =
      send "api/1/generic/private/order/add" [
        "type", "ask" 
        "amount_int", (qty * 100000000M |> round).ToString()
        "price_int", (price * 100000M |> round).ToString()
        ]  
    let! body = resp.Content.ReadAsStringAsync() |> Async.AwaitTask
    return body
    }

  member this.getOrders() = async {
    let! resp = send "api/1/generic/private/orders" []
    let! body = resp.Content.ReadAsStringAsync() |> Async.AwaitTask
    let json = JsonValue.Parse body
    if json?result.AsString() = "success" then
      let results = json?``return``.AsArray()
      for item in results do
        printfn "%s" (item.ToString())
    }
