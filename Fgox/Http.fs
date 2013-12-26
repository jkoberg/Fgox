module Fgox.Http

open System.Net.Http

type MtGoxAuthHandler(secrets: Fgox.Config.Secrets, innerHandler) = 
  inherit DelegatingHandler(innerHandler)
  let productHeader =
    let assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName()
    Headers.ProductInfoHeaderValue(assemblyName.Name, assemblyName.Version.ToString())

  member this.MySendAsync(req, token) = base.SendAsync(req, token) |> Async.AwaitTask

  override this.SendAsync(req, token) = Async.StartAsTask <| async {
      let! body = req.Content.ReadAsStringAsync() |> Async.AwaitTask
      let body = body + "&nonce=" + Fgox.Auth.getTonce().ToString()
      req.Headers.Add("Rest-Key", secrets.apikey)
      req.Headers.Add("Rest-Sign", Fgox.Auth.sign secrets body)
      req.Headers.UserAgent.Add(productHeader)
      req.Content <- new StringContent(body, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded")
      let! response = this.MySendAsync(req, token)
      return response
    }


type HttpClient with 
  static member fromGoxSecrets s =
    let this = new MtGoxAuthHandler(s, new HttpClientHandler())
    new HttpClient(this)
