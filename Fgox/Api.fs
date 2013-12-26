namespace Fgox

open System.Net.Http

open FSharp.Data.Json
open FSharp.Data.Json.Extensions

open Fgox.Config
open Fgox.Http


type Api(secrets:Secrets) =
  let priceFactor = 100000M
  let qtyFactor = 100000000M
  let client = HttpClient.fromGoxSecrets secrets
  let formEncode kvs =
    String.concat "&" <| [for k,v in kvs -> sprintf "%s=%s" k (System.Net.WebUtility.UrlEncode v)] 

  let send (pathsuffix:string) payload =  async {
    let reqbody = formEncode payload
    let! response =
      Async.AwaitTask <| client.PostAsync(
        System.Uri(secrets.uri, pathsuffix),
        new StringContent(reqbody)
        )
    let! respbody = response.Content.ReadAsStringAsync() |> Async.AwaitTask
    return (response, JsonValue.Parse respbody)
    }

  interface System.IDisposable with
    member x.Dispose() =
      client.Dispose()

  member this.sell (qty:decimal) (price:decimal) = async {
    let! resp, json =
      send "api/1/generic/private/order/add" [
        "type", "ask" 
        "amount_int", (qty * qtyFactor |> round).ToString()
        "price_int", (price * priceFactor |> round).ToString()
        ]
    return json
    }

  member this.getOrders() = async {
    let! resp, json = send "api/1/generic/private/orders" []
    return if json?result.AsString() = "success"
           then json?``return``.AsArray()
           else Array.empty
    }
