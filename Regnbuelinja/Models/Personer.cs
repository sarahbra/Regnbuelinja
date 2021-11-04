using System.ComponentModel.DataAnnotations;

namespace Regnbuelinja.Models
{
    public class Personer
    {
        public int Id { get; set; }
        [RegularExpression(@"^[a-zA-ZæøåÆØÅ]{2,30}$")]
        public string Fornavn { get; set; }
        [RegularExpression(@"^[a-zA-ZæøåÆØÅ]{2,30}$")]
        public string Etternavn { get; set; }
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z")]
        public string Epost { get; set; }
        [RegularExpression(@"^[0-9]{8}$")]
        public string Telefonnr { get; set; }
    }
}
