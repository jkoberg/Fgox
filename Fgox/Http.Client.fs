namespace Fgox.Http

open Fgox.Config
open System.Net
open System.Net.Http
open FSharp.Data.Json

type Client(secrets:Secrets) = 
  let nethttp = AuthHandler.MakeClient(secrets)

  let formEncode kvs =
      [for k,v in kvs -> sprintf "%s=%s" k (System.Net.WebUtility.UrlEncode v)] 
      |> String.concat "&"

  member this.send (pathsuffix:string) payload =  async {
    let reqbody = formEncode payload
    let uri = System.Uri(secrets.uri, pathsuffix)
    let! response = nethttp.PostAsync(uri, new StringContent(reqbody)) |> Async.AwaitTask
    let! respbody = response.Content.ReadAsStringAsync() |> Async.AwaitTask
    return (response, JsonValue.Parse respbody)
    }

  interface System.IDisposable with member x.Dispose() = nethttp.Dispose()
