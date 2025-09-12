using System.Text.RegularExpressions;

namespace Core.Application.Validators.Common;

    public static class ValidatorsTool
    {
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return Regex.IsMatch(name, @"^[A-Za-zÀ-ú\s]+$");
        }
        public static bool IsValidCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
                return false;

            // Elimina CPFs repetidos
            if (cpf.All(c => c == cpf[0]))
                return false;

            // Cálculo dos dígitos verificadores
            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;
            string digito = resto.ToString();
            tempCpf += digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;
            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }

        public static bool IsValidEmail(string email)
        {
            //return !string.IsNullOrWhiteSpace(email) &&
            //       Regex.IsMatch(
            //           email,
            //           @"^[^@\s]+@[^@\s]+\.[^@\s]+$"
            //       );

            if (string.IsNullOrWhiteSpace(email))
                return false;

            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }

        public static bool IsValidBirthDate(DateTime date)
        {
            if (date >= DateTime.Now)
                return false;

            var age = DateTime.Now.Year - date.Year;

            if (date > DateTime.Now.AddYears(-age)) age--;

            return age <= 120;
        }
    }