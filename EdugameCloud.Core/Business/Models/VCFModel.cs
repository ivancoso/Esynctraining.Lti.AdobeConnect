namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Castle.Core.Logging;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;
    using Thought.vCards;

    /// <summary>
    ///     The VCF model.
    /// </summary>
    public class VCFModel
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The country model.
        /// </summary>
        private readonly CountryModel countryModel;

        /// <summary>
        /// The state model.
        /// </summary>
        private readonly StateModel stateModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="VCFModel"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="countryModel">
        /// The country model.
        /// </param>
        /// <param name="stateModel">
        /// The state model.
        /// </param>
        public VCFModel(ILogger logger, CountryModel countryModel, StateModel stateModel)
        {
            this.logger = logger;
            this.countryModel = countryModel;
            this.stateModel = stateModel;
        }

        #region Public Methods and Operators

        /// <summary>
        /// The convert from VCF.
        /// </summary>
        /// <param name="vcfString">
        /// The vcf String.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ConvertFromVCF(string vcfString)
        {
            var sitesList = new List<string>();
            var vcf = new vCard(new StringReader(vcfString));
            dynamic x = new Xml();
            x.profile(
                Xml.Fragment(
                    profile =>
                        {
                            profile.@base(
                                Xml.Fragment(
                                    baze =>
                                        {
                                            baze.firstName(vcf.GivenName);
                                            baze.lastName(vcf.FamilyName);
                                            baze.jobTitle(vcf.Title);
                                            if (vcf.Notes != null && vcf.Notes.Any())
                                            {
                                                baze.about(
                                                    vcf.Notes.Aggregate(
                                                        string.Empty,
                                                        (current, note) => current + (note.Text + Environment.NewLine)));
                                            }
                                            else
                                            {
                                                baze.about(string.Empty);
                                            }
                                        }));

                            profile.contact(
                                new { isShared = "true" },
                                Xml.Fragment(
                                    contact =>
                                        {
                                            if (vcf.EmailAddresses != null && vcf.EmailAddresses.Any())
                                            {
                                                contact.email(
                                                    vcf.EmailAddresses.FirstOrDefault(e => e.IsPreferred)
                                                        .Return(
                                                            e => e.Address,
                                                            vcf.EmailAddresses.FirstOrDefault().With(e => e.Address)));
                                            }
                                            else
                                            {
                                                contact.email(string.Empty);
                                            }

                                            if (vcf.Phones != null && vcf.Phones.Any())
                                            {
                                                contact.phone(
                                                    vcf.Phones.FirstOrDefault(e => e.IsPreferred)
                                                        .Return(
                                                            e => e.FullNumber,
                                                            vcf.Phones.FirstOrDefault().With(e => e.FullNumber)));
                                            }
                                            else
                                            {
                                                contact.phone(string.Empty);
                                            }

                                            if (vcf.Websites != null && vcf.Websites.Any())
                                            {
                                                var webSite =
                                                    vcf.Websites.FirstOrDefault(
                                                        e => e.WebsiteType == vCardWebsiteTypes.Default)
                                                        .With(e => e.Url);
                                                if (!string.IsNullOrWhiteSpace(webSite))
                                                {
                                                    sitesList.Add(webSite);
                                                }
                                                contact.website(webSite);
                                            }
                                            else
                                            {
                                                contact.website(string.Empty);
                                            }
                                        }));

                                profile.location(
                                    new { isShared = "true" },
                                    Xml.Fragment(
                                        location =>
                                            {
                                                vCardDeliveryAddress address;
                                                    if (vcf.DeliveryAddresses != null && vcf.DeliveryAddresses.Any() && (address = vcf.DeliveryAddresses.FirstOrDefault(e => e.AddressType == vCardDeliveryAddressTypes.Default) ?? vcf.DeliveryAddresses.FirstOrDefault()) != null)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(address.Country))
                                                        {
                                                            object attributes = new { };
                                                            var countryEntity =
                                                                countryModel.GetOneByName(address.Country).Value;
                                                            if (countryEntity != null)
                                                            {
                                                                attributes = new { countryId = countryEntity.Id, countryCode3 = countryEntity.CountryCode3 };
                                                            }

                                                            location.country(attributes, address.Country);
                                                        }
                                                        else
                                                        {
                                                            location.country(new { countryId = string.Empty, countryCode3 = string.Empty }, string.Empty);
                                                        }

                                                        if (!string.IsNullOrWhiteSpace(address.Region))
                                                        {
                                                            object attributes = new { };
                                                            var stateEntity = stateModel.GetOneByName(address.Region).Value ?? stateModel.GetOneByCode(address.Region).Value;
                                                            if (stateEntity != null)
                                                            {
                                                                attributes = new { stateId = stateEntity.Id, stateCode = stateEntity.StateCode };
                                                            }

                                                            location.state(attributes, address.Region);
                                                        }
                                                        else
                                                        {
                                                            location.state(new { stateId = string.Empty }, string.Empty);
                                                        }

                                                        location.city(
                                                            string.IsNullOrWhiteSpace(address.City)
                                                                ? string.Empty
                                                                : address.City);
                                                        location.address(
                                                            string.IsNullOrWhiteSpace(address.Street)
                                                                ? string.Empty
                                                                : address.Street);
                                                        location.zip(
                                                            string.IsNullOrWhiteSpace(address.PostalCode)
                                                                ? string.Empty
                                                                : address.PostalCode);
                                                    }
                                                    else
                                                    {
                                                        location.country(string.Empty);
                                                        location.state(string.Empty);
                                                        location.city(string.Empty);
                                                        location.address(string.Empty);
                                                        location.zip(string.Empty);
                                                    }
                                            }));

                            profile.social(
                                Xml.Fragment(
                                    social =>
                                        {
                                            bool facebookFound = false,
                                                 twitterFound = false,
                                                 linkedInFound = false,
                                                 slideShareFound = false;
                                            if (vcf.Websites != null && vcf.Websites.Any(w => CheckSocial(w.Url)))
                                            {
                                                foreach (var webSite in vcf.Websites)
                                                {
                                                    if (CheckSocial(webSite.Url))
                                                    {
                                                        if (!facebookFound
                                                            && webSite.Url.ToLower().Contains("facebook.com"))
                                                        {
                                                            facebookFound = true;
                                                            social.facebook(webSite.Url);
                                                            sitesList.Add(webSite.Url);
                                                        }
                                                        else if (!twitterFound
                                                                 && webSite.Url.ToLower().Contains("twitter.com"))
                                                        {
                                                            twitterFound = true;
                                                            social.twitter(webSite.Url);
                                                            sitesList.Add(webSite.Url);
                                                        }
                                                        else if (!linkedInFound
                                                                 && webSite.Url.ToLower().Contains("linkedin.com"))
                                                        {
                                                            linkedInFound = true;
                                                            social.linkedin(webSite.Url);
                                                            sitesList.Add(webSite.Url);
                                                        }
                                                        else if (!slideShareFound
                                                                 && webSite.Url.ToLower()
                                                                        .Contains("slideshare.net"))
                                                        {
                                                            slideShareFound = true;
                                                            social.slideShare(webSite.Url);
                                                            sitesList.Add(webSite.Url);
                                                        }
                                                    }
                                                }
                                            }
                                            if (!facebookFound)
                                            {
                                                social.facebook();
                                            }
                                            if (!twitterFound)
                                            {
                                                social.twitter();
                                            }
                                            if (!linkedInFound)
                                            {
                                                social.linkedin();
                                            }
                                            if (!slideShareFound)
                                            {
                                                social.slideShare();
                                            }
                                        }));
                            profile.links(
                                new { isShared = "true" },
                                Xml.Fragment(
                                    links =>
                                        {
                                            foreach (var webSite in vcf.Websites)
                                            {
                                                if (!sitesList.Contains(webSite.Url))
                                                {
                                                    var url = webSite.Url;
                                                    links.link(
                                                        Xml.Fragment(
                                                            link =>
                                                                {
                                                                    link.linkLabel(string.Empty);
                                                                    link.url(url);
                                                                }));
                                                }
                                            }
                                        }));
                        }));
            return x.ToString(true);
        }

        /// <summary>
        /// The convert to VCF.
        /// </summary>
        /// <param name="xml">
        /// The XML.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public byte[] ConvertToVCF(string xml, out string fileName, out string exception)
        {
            fileName = null;
            exception = null;
            try
            {
                dynamic x = xml.ToDynamic();
                dynamic xmlRoot = x.profile;
                var vcfCard = new vCard();
                
                if (xmlRoot.@base != null)
                {
                    vcfCard.GivenName = xmlRoot.@base.firstName;
                    vcfCard.FamilyName = xmlRoot.@base.lastName;
                    vcfCard.GivenName = vcfCard.GivenName.Trim();
                    vcfCard.FamilyName = vcfCard.FamilyName.Trim();
                    var fullName = vcfCard.GivenName + vcfCard.FamilyName;
                    fileName = string.Format("{0}.vcf", fullName.Trim());
                    vcfCard.Title = xmlRoot.@base.jobTitle;

                    if (!string.IsNullOrWhiteSpace(xmlRoot.@base.about))
                    {
                        vcfCard.Notes.Add(xmlRoot.@base.about);
                    }
                }

                if (xmlRoot.contact != null)
                {
                    if (!string.IsNullOrWhiteSpace(xmlRoot.contact.email))
                    {
                        vcfCard.EmailAddresses.Add(new vCardEmailAddress(xmlRoot.contact.email));
                    }

                    if (!string.IsNullOrWhiteSpace(xmlRoot.contact.phone))
                    {
                        vcfCard.Phones.Add(new vCardPhone(xmlRoot.contact.phone));
                    }

                    if (!string.IsNullOrWhiteSpace(xmlRoot.contact.website))
                    {
                        vcfCard.Websites.Add(new vCardWebsite(xmlRoot.contact.website, vCardWebsiteTypes.Default));
                    }
                }
                if (xmlRoot.location != null)
                {
                    vcfCard.DeliveryAddresses.Add(
                        new vCardDeliveryAddress
                            {
                                AddressType = vCardDeliveryAddressTypes.Default,
                                Country = xmlRoot.location.country.value,
                                Region = xmlRoot.location.state.value,
                                City = xmlRoot.location.city,
                                Street = xmlRoot.location.address,
                                PostalCode = xmlRoot.location.zip,
                            });
                }

                if (xmlRoot.social != null)
                {
                    var fb = xmlRoot.social.facebook is string ? (string)xmlRoot.social.facebook : xmlRoot.social.facebook.value;
                    if (!string.IsNullOrWhiteSpace(fb))
                    {
                        vcfCard.Websites.Add(new vCardWebsite(fb, vCardWebsiteTypes.Personal));
                    }

                    var tw = xmlRoot.social.twitter is string ? (string)xmlRoot.social.twitter : xmlRoot.social.twitter.value;
                    if (!string.IsNullOrWhiteSpace(tw))
                    {
                        vcfCard.Websites.Add(new vCardWebsite(tw, vCardWebsiteTypes.Personal));
                    }

                    var li = xmlRoot.social.linkedin is string ? (string)xmlRoot.social.linkedin : xmlRoot.social.linkedin.value;
                    if (!string.IsNullOrWhiteSpace(li))
                    {
                        vcfCard.Websites.Add(new vCardWebsite(li, vCardWebsiteTypes.Personal));
                    }

                    var ss = xmlRoot.social.slideShare is string ? (string)xmlRoot.social.slideShare : xmlRoot.social.slideShare.value;
                    if (!string.IsNullOrWhiteSpace(ss))
                    {
                        vcfCard.Websites.Add(new vCardWebsite(ss, vCardWebsiteTypes.Personal));
                    }

                    var dic = xmlRoot.social as IDictionary<string, object>;
                    try
                    {
                        if (dic != null && dic.ContainsKey("blog"))
                        {

                            var bl = xmlRoot.social.blog is string
                                         ? (string)xmlRoot.social.blog
                                         : xmlRoot.social.blog.value;
                            if (!string.IsNullOrWhiteSpace(bl))
                            {
                                vcfCard.Websites.Add(new vCardWebsite(bl, vCardWebsiteTypes.Personal));
                            }

                        }
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        if (dic != null && dic.ContainsKey("selectedImageSocialSource"))
                        {
                            var source = (string)xmlRoot.social.@selectedImageSocialSource;
                            if (!string.IsNullOrWhiteSpace(source) && dic.ContainsKey(source))
                            {
                                var sourceDic = dic[source] as IDictionary<string, object>;
                                if (sourceDic != null && sourceDic.ContainsKey("imgUrl"))
                                {
                                    var imageRes = (string)sourceDic["imgUrl"];
                                    if (Uri.IsWellFormedUriString(imageRes, UriKind.Absolute))
                                    {
                                        vcfCard.Photos.Add(new vCardPhoto(imageRes));
                                    }
                                }
                            }
                            
                        }
                    }
                    catch(Exception)
                    {
                        
                    }

                }
                if (!(xmlRoot.links is string) && !((IDictionary<string, object>)xmlRoot.links).ContainsKey("value"))
                {
                    foreach (var link in xmlRoot.links.link)
                    {
                        if (!string.IsNullOrWhiteSpace(link.url))
                        {
                            vcfCard.Websites.Add(new vCardWebsite(link.url, vCardWebsiteTypes.Work));
                        }
                    }
                }

                var memory = new MemoryStream();
                var writer = new vCardStandardWriter { EmbedInternetImages = true };
                writer.Write(vcfCard, memory);
                return memory.ToArray();
            }
            catch (Exception ex)
            {
                this.logger.Error("Converting to VCF", ex);
                exception = ex.ToString();
                return null;
            }
        }

        /// <summary>
        /// The check social.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CheckSocial(string url)
        {
            return url.ToLower().Contains("facebook.com") || url.ToLower().Contains("twitter.com")
                   || url.ToLower().Contains("linkedin.com") || url.ToLower().Contains("slideshare.net");
        }

        #endregion
    }
}