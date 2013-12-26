module Fgox.Trader

open Fgox

let Run (gox:Fgox.Api) = async {
  let! result = gox.getOrders()
  printfn "Orders on your account:\n%A" result
  }
