﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using IdentityService.Model;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private Crypto encoder { get; set; }
        private string db_name { get; set; }
        private string connectionString { get; set; }
        private IConfigurationBuilder builder { get; set; }
        private IConfigurationRoot configuration { get; set; }
        public IdentityController()
        {
            encoder = new Crypto();
            builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            configuration = builder.Build();
            connectionString= configuration.GetSection("MongoSettings").GetSection("Connection").Value;
            db_name = configuration.GetSection("MongoSettings").GetSection("DatabaseName").Value;
        }


        [HttpPost]
        [Route("login")]
        public async Task<JsonResult> Login(string username, string password)
        {
            try
            {
                var client = new MongoClient(connectionString);
                var db = client.GetDatabase("etp_customer");
                IMongoCollection<User> collection = db.GetCollection<User>("user");
                var securepassword = encoder.HashHMAC(Encoding.ASCII.GetBytes("xUhs67g"), Encoding.ASCII.GetBytes(password));
                //SEEK FOR 'ACTIVE' USER
                var userResult = await collection.Find(x => x.Username == username && x.Password == securepassword).FirstOrDefaultAsync();


                if (userResult != null)
                {
                    //set status to true
                    var updateStatus = userResult;
                    updateStatus.OnlineStatus = true;
                    FilterDefinition<User> filter = Builders<User>.Filter.Eq("_id", updateStatus.Id);

                    await collection.ReplaceOneAsync(filter, updateStatus);

                    var USER_ID = userResult.Id.ToString();
                    encoder = new Crypto();

                    var now = DateTime.UtcNow;
                    //SET USER CLAIMS
                    var claims = new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, username),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(),ClaimValueTypes.Integer64),
                        new Claim("etp_user",encoder.Encrypt(USER_ID))
                    };
                    //Define SymmetricSecurityKey
                    var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("a464ce52555fd73023f47d396ab9db20"));
                    var tokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        ValidateIssuer = true,
                        ValidIssuer = "localhost",
                        ValidateAudience = true,
                        ValidAudience = "Solar Banker",
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        RequireExpirationTime = true
                    };
                    //CREATE the token
                    var jwt = new JwtSecurityToken
                    (
                        issuer: "localhost",
                        audience: "Solar Banker",
                        claims: claims,
                        notBefore: now,
                        expires: now.Add(TimeSpan.FromMinutes(30)),
                        signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                    );

                    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                    var responseJson = new
                    {
                        access_token = encodedJwt,
                        expires_in = (int)TimeSpan.FromMinutes(30).TotalSeconds
                    };

                    return new JsonResult(responseJson);
                }
                else
                {
                    var failResponse = new
                    {
                        successCode = 0,
                        message = "User does not exist or wrong information!"
                    };
                    return new JsonResult(failResponse);
                }
            }
            catch (Exception e)
            {
                var errorJson = new
                {
                    error = e.ToString(),
                    error_message = e.Message
                };

                return new JsonResult(errorJson);
            }

        }
    }
}