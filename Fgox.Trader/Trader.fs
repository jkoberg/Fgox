module Fgox.Trader

open Fgox

let execTrades (gox:Fgox.Api) = async {
  let! ticket = gox.getOrders()
  printfn "Sell entered: %A" ticket
  }
