namespace Fgox.Types

type ResponseResult = Error | Success

type ResponseMessage = {
  result: string
  ret: FSharp.Data.Json.JsonValue
  }



