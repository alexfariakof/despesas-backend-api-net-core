﻿namespace Business.Dtos.Core;
public abstract class BaseControleAcessoDto : BaseUsuarioDto
{
    public virtual string? Senha { get; set; }
    public virtual string? ConfirmaSenha { get; set; }
    public virtual string? RefreshToken { get; set; }
}