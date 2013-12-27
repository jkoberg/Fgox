namespace Fgox

open System.Net.Http
open FSharp.Data.Json
open FSharp.Data.Json.Extensions
open Fgox.Config

open Fgox.Types.Units

type Api(secrets:Secrets) =
  let http = new Fgox.Http.Client(secrets)
  let priceFactor = 10e5M</dollar> 
  let qtyFactor = 1M</satoshi>

  member api.Sell (qty:Btc) (price:Dollar) = async {
    let! resp, json =
      http.send "api/1/generic/private/order/add" [
        "type", "ask" 
        "amount_int", (btcToSatoshi qty * qtyFactor |> round).ToString()
        "price_int", (price * priceFactor |> round).ToString()
        ]
    return json
    }

  member api.GetOrders () = async {
    let! resp, json = http.send "api/1/generic/private/orders" []
    if json?result.AsString() = "success" then
      return json?``return``.AsArray()
    else 
      return Array.empty
    }
