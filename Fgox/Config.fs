namespace Fgox.Config
open FSharp.Data.Json
open FSharp.Data.Json.Extensions
open System

module AsyncFile =
  let read filename = async {
    use bodystream  = new IO.FileStream(path=filename, mode=IO.FileMode.Open)
    use reader = new IO.StreamReader(bodystream)
    return! reader.ReadToEndAsync() |> Async.AwaitTask
    }

  let write filename (data:string) = async {
    use bodystream  = new IO.FileStream(path=filename, mode=IO.FileMode.Create)
    use writer = new IO.StreamWriter(bodystream)
    let! flag = Async.AwaitIAsyncResult <| writer.WriteAsync data
    ignore flag
    }


type Secrets = {
  uri: Uri
  apikey: string
  secret: byte[]
  }
with
  static member ofJson body =
    let raw = JsonValue.Parse(body)
    { uri = System.Uri(raw?uri.AsString())
      apikey = raw?apikey.AsString()
      secret = raw?secret.AsString() |> Convert.FromBase64String
      }

  static member fromFile f = async {
    let! data = AsyncFile.read f
    return Secrets.ofJson data
    }

  member this.asJson = 
    (JsonValue.Object <| Map [
      "uri", this.uri.ToString() |> JsonValue.String
      "apikey", this.apikey |> JsonValue.String 
      "secret", this.secret |> Convert.ToBase64String |> JsonValue.String
    ]).ToString()

  member this.toFile f = async {
    do! AsyncFile.write f this.asJson
    }


module Tests =
  open NUnit.Framework

  let body = """
  {
    "uri": "https://data.mtgox.com/api/2/base",
    "apikey": "abc",
    "secret": "ZGVm"
  }
  """

  let expected = {uri=Uri("https://data.mtgox.com/api/2/base"); apikey="abc"; secret="def"B}

  let [<Test>] ``deserialize and reserialize a valid secret config body`` () =
    let actual = Secrets.ofJson body
    Assert.AreEqual(expected, actual)
    let reserialized = actual.asJson
    let actual2 = Secrets.ofJson reserialized
    Assert.AreEqual(expected, actual2)



  
  


