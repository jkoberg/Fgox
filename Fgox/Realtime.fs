module Fgox.Realtime

open System
open System.Collections.Generic
open FSharp.Data.Json
open FSharp.Data.Json.Extensions
open WebSocket4Net
open Fgox.Types.Units

type Trade = 
  { id: string
    at: DateTimeOffset
    qty: Btc
    price: Dollar
    }
    
let tradeEpoch = DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)

let parseJsonMessage (e:MessageReceivedEventArgs) = JsonValue.Parse e.Message
  
let tradeChannelId = Guid("dbf1dee9-4f2e-4a08-8cb7-748919a71b21")

let isTradeMsg (j:JsonValue) =
  j?op.AsString() = "private" && j?channel.AsGuid() = tradeChannelId

let parseTradeMsg (j:JsonValue) =
  let t = j?trade
  { id = t?tid.AsString()
    at = tradeEpoch.AddSeconds <| t?date.AsFloat()
    qty = satoshiToBtc <| t?amount_int.AsDecimal() / Fgox.Api.qtyFactor 
    price = t?price_int.AsDecimal() / Fgox.Api.priceFactor
    }

type GoxSocket(socketurl) =
  let url = "https://websocket.mtgox.com/mtgox"
  let websocket = new WebSocket(url)

  member this.Trades =
    websocket.MessageReceived
    |> Observable.map parseJsonMessage
    |> Observable.filter isTradeMsg
    |> Observable.map parseTradeMsg


let Test() = 
  let socket = GoxSocket("")
  socket.Trades.Add(fun t -> printfn "%A" t)