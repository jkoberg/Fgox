module Fgox.Trader

open Fgox

let execTrades (gox:Fgox.Api) = async {
  let! ticket = gox.sell 1.0M 800.0M
  printfn "Sell entered: %A" ticket
  }
