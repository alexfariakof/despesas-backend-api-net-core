﻿namespace Domain.ViewModel
{
    public class ChangePasswordVMTest
    {
        [Theory]
        [InlineData("userTeste1", "userTeste1")]
        [InlineData("userTeste2", "userTeste2")]
        [InlineData("userTeste3", "userTeste3")]
        public void ChangePasswordVM_Should_Set_Properties_Correctly(string senha, string confirmaSenha)
        {
            // Arrange and Act
            var changePasswordVM = new ChangePasswordVM
            {
                Senha = senha,
                ConfirmaSenha = confirmaSenha
            };

            // Assert
            Assert.Equal(senha, changePasswordVM.Senha);
            Assert.Equal(confirmaSenha, changePasswordVM.ConfirmaSenha);
        }
    }
}