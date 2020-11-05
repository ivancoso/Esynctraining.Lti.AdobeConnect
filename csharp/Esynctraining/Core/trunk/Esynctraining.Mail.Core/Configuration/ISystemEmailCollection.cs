namespace Esynctraining.Mail.Configuration
{
    public interface ISystemEmailCollection
    {
        ISystemEmail GetByToken(string emailToken);

    }

}
