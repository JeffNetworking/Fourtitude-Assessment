using Application.Helper;
using Application.Model;
using Application.Respone;
using log4net;
using MediatR;
using System.Reflection;
using System.Text;

namespace Application.Command
{
    public class TransactionCommandHandler : IRequestHandler<TransactionCommand, TransactionRespone>
    {
        private TransactionCommand _command;
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
        private static readonly new List<Partner>  partners = new List<Partner>
        {
            new Partner { PartnerNo = "FG-00001", AllowedPartner = "FAKEGOOGLE", PartnerPassword = "FAKEPASSWORD1234" },
            new Partner { PartnerNo = "FG-00002", AllowedPartner = "FAKEPEOPLE", PartnerPassword = "FAKEPASSWORD4578" }
        };
       
        public async Task<TransactionRespone> Handle(TransactionCommand command, CancellationToken cancellationToken)
        {
            Init(command);
            Validate();

            var (totalDiscount, finalAmount) = HelperModule.CalculateDiscount(_command.TotalAmount);

            var response = new TransactionRespone
            {
                Result = 1,
                TotalAmount = _command.TotalAmount,
                TotalDiscount = totalDiscount,
                FinalAmount = finalAmount
            };

            return response;
        }

        private void Init(TransactionCommand command)
        {
            _command = command;
            _logger.Info("initialize command");
        }

        private void Validate()
        {
            ValidateRequest();
            ValidatePartner();
            ValidateComputeSignature();
        }
        private void ValidateRequest()
        {
            _logger.Info("Validate Request");
            _command.Validate();
        }

        private void ValidatePartner()
        {
            _logger.Info("Validate Partner");
            var partner = partners.FirstOrDefault(p => p.PartnerNo == _command.PartnerrefNo);

            if (partner == null ||
                Convert.ToBase64String(Encoding.UTF8.GetBytes(partner.PartnerPassword)) != _command.PartnerPassword)
            {
                _logger.Info("Validate Partner Fail");
                throw new Exception("Access Denied!");
            }
        }


        private void ValidateComputeSignature()
        {
            var sig = HelperModule.ComputeSignature(
                        _command.TimeStamp,
                        _command.PartnerKey,
                        _command.PartnerrefNo,
                        _command.TotalAmount,
                        _command.PartnerPassword
                        );


            if (sig != _command.Sig)
            {
                _logger.Info("Validate Signature Fail");

                throw new Exception("Access Denied!");
            }

            _logger.Info("Validate Signature Success");
        }
    }
}
