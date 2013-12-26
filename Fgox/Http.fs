module Fgox.Http

open System.Net.Http

let formEncode kvs = 
  [for k,v in kvs -> sprintf "%s=%s" k (System.Net.WebUtility.UrlEncode v)] |> String.concat "&"

let assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName()
let productHeader = Headers.ProductInfoHeaderValue(assemblyName.Name, assemblyName.Version.ToString())

let addNonce ps = ("nonce", Fgox.Auth.getTonce().ToString()) :: ps

let makePost (secrets:Fgox.Config.Secrets) (pathsuffix:string) payload =
  let body = payload |> addNonce |> formEncode
  let req = new HttpRequestMessage(HttpMethod.Post, System.Uri(secrets.uri, pathsuffix))
  req.Headers.Add("Rest-Key", secrets.apikey)
  req.Headers.Add("Rest-Sign", Fgox.Auth.sign secrets body)
  req.Headers.UserAgent.Add(productHeader)
  req.Content <- new StringContent(body, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded")
  req

