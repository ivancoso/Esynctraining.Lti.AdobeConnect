namespace Esynctraining.AC.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Esynctraining.AC.Provider.Constants;
    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.EntityParsing;
    using Esynctraining.AC.Provider.Utils;

    /// <summary>
    /// The adobe connect provider.
    /// </summary>
    public partial class AdobeConnectProvider
    {
        #region Private Constants

        private const string ScoHome = "//sco";
        private const string ScoNavHome = "//sco-nav";

        #endregion

        public ScoContentCollectionResult SearchScoByName(string name)
        {
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.Sco.SearchByField, string.Format(CommandParams.FieldAndQuery, "name", name), out status);

            return ResponseIsOk(doc, status)
                       ? new ScoContentCollectionResult(status, ScoSearchByFieldParser.Parse(doc))
                       : new ScoContentCollectionResult(status);
        }

        /// <summary>
        /// The search SCO by description.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <returns>
        /// The <see cref="ScoContentCollectionResult"/>.
        /// </returns>
        public ScoContentCollectionResult SearchScoByDescription(string description)
        {
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.Sco.SearchByField, string.Format(CommandParams.FieldAndQuery, "description", description), out status);

            return ResponseIsOk(doc, status)
                       ? new ScoContentCollectionResult(status, ScoSearchByFieldParser.Parse(doc))
                       : new ScoContentCollectionResult(status);
        }
        
        /// <summary>
        /// The get SCO info.
        /// </summary>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="ScoInfoResult"/>.
        /// </returns>
        public ScoInfoResult GetScoInfo(string scoId)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));

            // act: "sco-info"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.Sco.Info, string.Format(CommandParams.ScoId, scoId), out status);

            return ResponseIsOk(doc, status)
                ? new ScoInfoResult(status, ScoInfoParser.Parse(doc.SelectSingleNode(ScoHome)))
                : new ScoInfoResult(status);
        }

        /// <summary>
        /// The get SCO info.
        /// </summary>
        /// <param name="scoUrl">
        /// The SCO url.
        /// </param>
        /// <returns>
        /// The <see cref="ScoInfoResult"/>.
        /// </returns>
        public ScoInfoResult GetScoByUrl(string scoUrl)
        {
            if (string.IsNullOrWhiteSpace(scoUrl))
                throw new ArgumentException("Non-empty value expected", nameof(scoUrl));

            // act: "sco-by-url"
            StatusInfo status;

            var pathId = scoUrl.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            var urlPath = string.Format("/{0}/", pathId);
            var doc = this.requestProcessor.Process(Commands.Sco.ByUrl, string.Format(CommandParams.UrlPath, urlPath), out status);

            return ResponseIsOk(doc, status)
                ? new ScoInfoResult(status, ScoInfoParser.Parse(doc.SelectSingleNode(ScoHome)))
                : new ScoInfoResult(status);
        }

        /// <summary>
        /// The get contents by SCO id.
        /// </summary>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="ScoContentCollectionResult"/>.
        /// </returns>
        public ScoContentCollectionResult GetContentsByScoId(string scoId)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));

            StatusInfo status;

            // TRICK: http://www.connectusers.com/forums/topic/8827/adobe-connect-8-web-services-bytecount-missing-scocontents/
            var scos = this.requestProcessor.Process(Commands.Sco.Contents, string.Format(CommandParams.ScoId + "&counters=true", scoId), out status);

            return ResponseIsOk(scos, status)
                ? new ScoContentCollectionResult(status, ScoContentCollectionParser.Parse(scos), scoId)
                : new ScoContentCollectionResult(status);
        }

        public ScoContentCollectionResult GetContentsByScoId(string scoId, string filterName, string filterType)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));

            StatusInfo status;

            var scos = this.requestProcessor.Process(Commands.Sco.Contents,
                string.Format(CommandParams.ScoNameAndType, scoId, UrlEncode(filterName), UrlEncode(filterType)),
                out status);

            return ResponseIsOk(scos, status)
                ? new ScoContentCollectionResult(status, ScoContentCollectionParser.Parse(scos), scoId)
                : new ScoContentCollectionResult(status);
        }
        
        /// <summary>
        /// The get contents by SCO id.
        /// </summary>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="ScoContentCollectionResult"/>.
        /// </returns>
        public ScoContentResult GetScoContent(string scoId)
        {
            StatusInfo status;

            var scos = this.requestProcessor.Process(Commands.Sco.Info, string.Format(CommandParams.ScoId, scoId), out status);

            // ReSharper disable once InconsistentNaming
            const string scoPath = "//results/sco";

            return ResponseIsOk(scos, status)
                // ReSharper disable once AssignNullToNotNullAttribute
                ? new ScoContentResult(status, ScoContentParser.Parse(scos.SelectNodes(scoPath).Cast<XmlNode>().FirstOrDefault()))
                : new ScoContentResult(status);
        }

        /// <summary>
        /// The get contents by SCO id.
        /// </summary>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="ScoContentCollectionResult"/>.
        /// </returns>
        public ScoContentCollectionResult GetScoExpandedContent(string scoId)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));

            var filter = string.Format(CommandParams.ScoId, scoId);
            return CallScoExpandedContent(scoId, filter);
        }

        public ScoContentCollectionResult GetScoExpandedContentByName(string scoId, string name)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));
            // ???
            //if (string.IsNullOrWhiteSpace(name))
            //    throw new ArgumentException("Non-empty value expected", nameof(name));

            var filter = string.Format(CommandParams.ScoName, scoId, UrlEncode(name));
            return CallScoExpandedContent(scoId, filter);
        }

        public ScoContentCollectionResult GetScoExpandedContentByNameLike(string scoId, string nameLikeCriteria)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));
            // ???
            //if (string.IsNullOrWhiteSpace(nameLikeCriteria))
            //    throw new ArgumentException("Non-empty value expected", nameof(nameLikeCriteria));

            var filter = string.Format(CommandParams.ScoNameLike, scoId, UrlEncode(nameLikeCriteria)); ;
            return CallScoExpandedContent(scoId, filter);
        }

        public CollectionResult<ScoNav> GetScoNavigation(string scoId)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));

            return GetCollection(Commands.Sco.ScoNav, "sco-id=" + scoId, ScoNavHome, "//sco",
                ScoNavParser.Parse);
        }

        private ScoContentCollectionResult CallScoExpandedContent(string scoId, string filter)
        {
            StatusInfo status;

            var scos = this.requestProcessor.Process(Commands.Sco.ExpandedContents, filter, out status);

            // ReSharper disable once InconsistentNaming
            const string scoPath = "//expanded-scos/sco";

            return ResponseIsOk(scos, status)
                ? new ScoContentCollectionResult(status, ScoContentCollectionParser.Parse(scos, scoPath), scoId)
                : new ScoContentCollectionResult(status);
        }


        #region Write

        /// <summary>
        /// The create SCO.
        /// </summary>
        /// <typeparam name="T">
        /// SCO update item.
        /// </typeparam>
        /// <param name="scoUpdateItem">
        /// The SCO update item.
        /// </param>
        /// <returns>
        /// The <see cref="ScoInfoResult"/>.
        /// </returns>
        public ScoInfoResult CreateSco<T>(T scoUpdateItem)
            where T : ScoUpdateItemBase
        {
            if (scoUpdateItem == null)
            {
                return new ScoInfoResult(new StatusInfo { Code = StatusCodes.internal_error });
            }

            if (string.IsNullOrEmpty(scoUpdateItem.FolderId))
            {
                return new ScoInfoResult(CreateStatusInfo(StatusCodes.invalid, StatusSubCodes.format, new ArgumentNullException("scoUpdateItem", "FolderId must be set to create new item")));
            }

            if (scoUpdateItem.Type == ScoType.not_set)
            {
                return new ScoInfoResult(CreateStatusInfo(StatusCodes.invalid, StatusSubCodes.format, new ArgumentNullException("scoUpdateItem", "ScoType must be set")));
            }

            scoUpdateItem.ScoId = null;

            return this.ScoUpdate(scoUpdateItem, false);
        }

        /// <summary>
        /// The update SCO.
        /// </summary>
        /// <typeparam name="T">
        /// SCO update item.
        /// </typeparam>
        /// <param name="scoUpdateItem">
        /// The SCO update item.
        /// </param>
        /// <returns>
        /// The <see cref="ScoInfoResult"/>.
        /// </returns>
        public ScoInfoResult UpdateSco<T>(T scoUpdateItem)
            where T : ScoUpdateItemBase
        {
            if (scoUpdateItem == null)
            {
                return new ScoInfoResult(new StatusInfo { Code = StatusCodes.internal_error });
            }

            if (string.IsNullOrEmpty(scoUpdateItem.ScoId))
            {
                return new ScoInfoResult(CreateStatusInfo(StatusCodes.invalid, StatusSubCodes.format, new ArgumentNullException("scoUpdateItem", "scoId must be set to update existing item")));
            }

            scoUpdateItem.FolderId = null;

            return this.ScoUpdate(scoUpdateItem, true);
        }

        /// <summary>
        /// Deletes one or more objects (SCOs).
        /// If the SCO-id you specify is for a folder, all the contents of the specified folder are deleted. To
        /// delete multiple SCOs, specify multiple SCO-id parameters.
        /// You can use a call such as SCO-contents to check the ref-count of the SCO, which is the
        /// number of other SCOs that reference this SCO. If the SCO has no references, you can safely
        /// remove it, and the server reclaims the space.
        /// If the SCO has references, removing it can cause the SCOs that reference it to stop working,
        /// or the server not to reclaim the space, or both. For example, if a course references a quiz
        /// presentation, removing the presentation might make the course stop working.
        /// As another example, if a meeting has used a content SCO (such as a presentation or video),
        /// there is a reference from the meeting to the SCO. Deleting the content SCO does not free
        /// disk space, because the meeting still references it.
        /// To delete a SCO, you need at least manage permission (see permission-id for details). Users
        /// who belong to the built-in authors group have manage permission on their own content
        /// folder, so they can delete content within it.
        /// </summary>
        /// <param name="scoId">The SCO id.</param>
        /// <returns>Status Info.</returns>
        public StatusInfo DeleteSco(string scoId)
        {
            StatusInfo status;

            this.requestProcessor.Process(Commands.Sco.Delete, string.Format(CommandParams.ScoId, scoId), out status);

            return status;
        }

        /// <summary>
        /// The move SCO.
        /// </summary>
        /// <param name="folderId">
        /// The folder id.
        /// </param>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="StatusInfo"/>.
        /// </returns>
        public StatusInfo MoveSco(string folderId, string scoId)
        {
            StatusInfo status;

            this.requestProcessor.Process(Commands.Sco.Move, string.Format(CommandParams.Move, folderId, scoId), out status);
            return status;
        }
        
        #endregion

        #region internal routines
        
        public IEnumerable<ScoShortcut> GetShortcuts(out StatusInfo status)
        {
            var shortcuts = this.requestProcessor.Process(Commands.Sco.Shortcuts, null, out status);

            return !ResponseIsOk(shortcuts, status) ? null : ShortcutCollectionParser.Parse(shortcuts);
        }

        /// <summary>
        /// The get shortcut by type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="ScoShortcut"/>.
        /// </returns>
        public ScoShortcut GetShortcutByType(string type, out StatusInfo status)
        {
            var shortcuts = this.requestProcessor.Process(Commands.Sco.Shortcuts, null, out status);

            return !ResponseIsOk(shortcuts, status) ? null : ShortcutCollectionParser.GetByType(shortcuts, type);
        }

        /// <summary>
        /// Creates metadata for a SCO, or updates existing metadata describing a SCO.
        /// Call SCO-update to create metadata only for SCOs that represent content, including
        /// meetings. You also need to upload content files with either SCO-upload or Connect Enterprise Manager.
        /// You must provide a folder-id or a SCO id, but not both. If you pass a folder-id, SCO-update
        /// creates a new SCO and returns a SCO id. If the SCO already exists and you pass a
        /// SCO-id, SCO-update updates the metadata describing the SCO.
        /// After you create a new SCO with SCO-update, call permissions-update to specify which
        /// users and groups can access it.
        /// </summary>
        /// <typeparam name="T">
        /// Base update item.
        /// </typeparam>
        /// <param name="meetingUpdateItem">
        /// The meeting item.
        /// </param>
        /// <param name="isUpdate">
        /// Is Update.
        /// </param>
        /// <returns>
        /// Save Meeting Result.
        /// </returns>
        private ScoInfoResult ScoUpdate<T>(T meetingUpdateItem, bool isUpdate)
            where T : ScoUpdateItemBase
        {
            if (meetingUpdateItem == null)
            {
                return null;
            }

            //if (string.IsNullOrEmpty(scoUpdateItem.FolderId))
            //{
            //    return new ScoInfoResult(CreateStatusInfo(StatusCodes.invalid, StatusSubCodes.format, new ArgumentNullException("scoUpdateItem", "FolderId must be set to create new item")));
            //}

            var commandParams = QueryStringBuilder.EntityToQueryString(meetingUpdateItem);

            StatusInfo status;
            var doc = this.requestProcessor.Process(Commands.Sco.Update, commandParams, out status);

            if (!ResponseIsOk(doc, status))
            {
                return new ScoInfoResult(status);
            }

            if (isUpdate)
            {
                return this.GetScoInfo(meetingUpdateItem.ScoId);
            }

            // notice: no '/sco' will be returned during update
            var detailNode = doc.SelectSingleNode(ScoHome);

            if (detailNode == null || detailNode.Attributes == null)
            {
                return new ScoInfoResult(status);
            }

            ScoInfo meetingDetail = null;

            try
            {
                meetingDetail = ScoInfoParser.Parse(detailNode);
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
                status.Code = StatusCodes.invalid;
                status.SubCode = StatusSubCodes.format;
                status.UnderlyingExceptionInfo = ex;

                // delete meeting
                // [DD]: why would you do that?!..
                // if (meetingDetail != null && !string.IsNullOrEmpty(meetingDetail.scoId))
                // {
                // this.DeleteSco(meetingDetail.scoId);
                // }
            }

            return new ScoInfoResult(status, meetingDetail);
        }

        #endregion

    }

}
