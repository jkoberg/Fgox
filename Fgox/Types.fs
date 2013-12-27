namespace Fgox.Types

type ResponseResult = Error | Success

type ResponseMessage = {
  result: string
  ret: FSharp.Data.Json.JsonValue
  }


module Units = 
  [<Measure>] type satoshi
  type Satoshi = decimal<satoshi>

  [<Measure>] type btc
  type Btc = decimal<btc>
  
  let btcToSatoshi (b:Btc) : Satoshi= b * 10e8M<satoshi/btc>

  let satoshiToBtc (s:Satoshi) : Btc = s * 10e-8M<btc/satoshi>

  [<Measure>] type dollar
  type Dollar = decimal<dollar>

