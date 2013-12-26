module Fgox.Auth

open System
open System.Text
open System.Security.Cryptography

/// The standard unix epoch
let unixEpoch = DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)

/// Derive the MtGox authentication `tonce` value from the Windows clock by
/// adjusting to the unix epoch and dividing the 100ns absolute tick count
/// by 10 to yield microseconds-since-epoch.
let getTonce () = (DateTimeOffset.UtcNow.Ticks - unixEpoch.Ticks) / 10L

//let getRand count = 
//  let nonce : byte[] = Array.zeroCreate count
//  use rng = new RNGCryptoServiceProvider()
//  rng.GetBytes(nonce)
//  nonce

let hmac512sign (key:byte[]) (body:string) =
  let data = Encoding.UTF8.GetBytes(body)
  let h = new HMACSHA512(key)
  h.ComputeHash(data)

let hmac512verify k d s = s = hmac512sign k d

let sign (secrets:Config.Secrets) (reqBody:string) =
  hmac512sign secrets.secret reqBody |> Convert.ToBase64String
