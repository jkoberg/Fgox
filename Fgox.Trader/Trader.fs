module Fgox.Trader

open Fgox

let Run (gox:Fgox.Api) = async {
  let! result = gox.GetOrders()
  printfn "Orders on your account:\n%A" result
  }
