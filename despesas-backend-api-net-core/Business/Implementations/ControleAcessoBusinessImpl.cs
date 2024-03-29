﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using despesas_backend_api_net_core.Domain.Entities;
using despesas_backend_api_net_core.Infrastructure.Data.Repositories;
using despesas_backend_api_net_core.Infrastructure.Security.Configuration;
using despesas_backend_api_net_core.Infrastructure.Data.EntityConfig;

namespace despesas_backend_api_net_core.Business.Implementations
{
    public class ControleAcessoBusinessImpl : IControleAcessoBusiness
    {
        private IControleAcessoRepositorio _repositorio;

        private SigningConfigurations _singingConfiguration;
        private TokenConfiguration _tokenConfiguration;
        private readonly ControleAcessoMap _converter;

        public ControleAcessoBusinessImpl(IControleAcessoRepositorio repositorio, SigningConfigurations singingConfiguration, TokenConfiguration tokenConfiguration)
        {
            _repositorio = repositorio;
            _singingConfiguration = singingConfiguration;
            _tokenConfiguration = tokenConfiguration;
        }
        public bool Create(ControleAcesso controleAcesso)
        {
            return _repositorio.Create(controleAcesso);
        }
        public Authentication FindByLogin(ControleAcesso controleAcesso)
        {
            bool credentialsValid = false;

            var usuario = _repositorio.GetUsuarioByEmail(controleAcesso.Login);
            if (usuario == null)
                return ExceptionObject("Usuário inexistente!");
            else if (usuario.StatusUsuario == StatusUsuario.Inativo)
                return ExceptionObject("Usuário Inativo!");

            if (controleAcesso != null && !string.IsNullOrWhiteSpace(controleAcesso.Login))
            {
                ControleAcesso baseLogin = _repositorio.FindByEmail(controleAcesso);
                if (baseLogin == null)
                    return ExceptionObject("Email inexistente!");
                if (!_repositorio.isValidPasssword(controleAcesso))
                    return ExceptionObject("Senha inválida!");

                credentialsValid = baseLogin != null && controleAcesso.Login == baseLogin.Login && _repositorio.isValidPasssword(controleAcesso);
            }
            if (credentialsValid)
            {
                ClaimsIdentity identity = new ClaimsIdentity(
                    new GenericIdentity(controleAcesso.Login, "Login"),
                    new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, controleAcesso.Login)
                    });

                DateTime createDate = DateTime.Now;
                DateTime expirationDate = createDate + TimeSpan.FromSeconds(_tokenConfiguration.Seconds);

                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                string token = CreateToken(identity, createDate, expirationDate, handler, usuario.Id);
                return SuccessObject(createDate, expirationDate, token, controleAcesso.Login);
            }
            return ExceptionObject("Usuário Inválido!");
        }
        public bool RecoveryPassword(string email)
        {
            return _repositorio.RecoveryPassword(email);
        }

        public bool ChangePassword(int idUsuario, string password)
        {
            return _repositorio.ChangePassword(idUsuario, password);
        }
        private string CreateToken(ClaimsIdentity identity, DateTime createDate, DateTime expirationDate, JwtSecurityTokenHandler handler, int idUsuario)
        {
            Microsoft.IdentityModel.Tokens.SecurityToken securityToken = handler.CreateToken(new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Issuer = _tokenConfiguration.Issuer,
                Audience = _tokenConfiguration.Audience,
                SigningCredentials = _singingConfiguration.SigningCredentials,
                Subject = identity,
                NotBefore = createDate,
                Expires = expirationDate,
                Claims = new Dictionary<string, object> { { "IdUsuario", idUsuario } },
            });

            string token = handler.WriteToken(securityToken);

            
            return token;
        }
        private Authentication ExceptionObject(string message)
        {
            return new Authentication
            {
                Authenticated = false,
                Message = message
            };
        }
        private Authentication SuccessObject(DateTime createDate, DateTime expirationDate, string token, string login)
        {
            Usuario usuario = _repositorio.GetUsuarioByEmail(login);
            return new Authentication
            {
                Authenticated = true,
                Created = createDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Expiration = expirationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                AccessToken = token,
                Message = "OK"
            };
        }
    }
}
