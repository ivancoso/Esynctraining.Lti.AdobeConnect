namespace eSyncTraining.Web.Controllers
{
    using System.Web.Mvc;

    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using eSyncTraining.Web.Models;

    /// <summary>
    /// The sco update controller base.
    /// </summary>
    public abstract class ScoUpdateControllerBase<T, E> : AdobeConnectedControllerBase
        where T : ScoDetailModelBase, new()
        where E : ScoUpdateItemBase
    {
        /// <summary>
        /// Gets the view name.
        /// </summary>
        protected abstract string ViewName { get; }

        /// <summary>
        /// The get model.
        /// </summary>
        /// <param name="scoInfoResult">
        /// The sco info.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        protected abstract T GetModel(ScoInfoResult scoInfoResult);

        /// <summary>
        /// The prepare view data.
        /// </summary>
        protected abstract void PrepareViewData();

        /// <summary>
        /// The model to update entity.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="E"/>.
        /// </returns>
        protected abstract E ModelToUpdateEntity(T model);

        /// <summary>
        /// Post-Sco-update routine.
        /// Called only if sco-update succeeded!
        /// </summary>
        /// <param name="scoId">The sco id.</param>
        /// <param name="model">The model.</param>
        protected abstract void SaveAdditionalData(string scoId, T model);

        #region Base implementation

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="folderId">
        /// The folder id.
        /// </param>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Create(string folderId, string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;

            if (!this.LoginWithSession().Success)
            {
                return this.RedirectToLocal(null);
            }

            this.PrepareViewData();

            return this.View(this.ViewName, new T { FolderId = folderId });
        }

        /// <summary>
        /// The edit.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Edit(string id, string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;

            if (!this.LoginWithSession().Success)
            {
                return this.RedirectToLocal(null);
            }

            this.PrepareViewData();

            var scoInfo = this.AdobeConnect.GetScoInfo(id);

            if (!scoInfo.Success)
            {
                return this.RedirectToLocal(null);
            }

            return this.View(this.ViewName, this.GetModel(scoInfo));
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public ActionResult Save(T model, string returnUrl)
        {
            try
            {
                if (this.LoginWithSession().Success)
                {
                    var updateItem = this.ModelToUpdateEntity(model);

                    var scoId = string.IsNullOrWhiteSpace(updateItem.ScoId) ? null : updateItem.ScoId;
                    var saveSucceeded = false;

                    if (string.IsNullOrWhiteSpace(scoId))
                    {
                        var result = this.AdobeConnect.CreateSco(updateItem);

                        if (result.Success && result.ScoInfo != null)
                        {
                            scoId = result.ScoInfo.ScoId;
                            saveSucceeded = true;
                        }
                    }
                    else
                    {
                        saveSucceeded = this.AdobeConnect.UpdateSco(updateItem).Success;
                    }

                    if (saveSucceeded)
                    {
                        this.SaveAdditionalData(scoId, model);

                        return this.RedirectToAction("Edit", new { id = scoId, returnUrl });
                    }
                }
            }
            catch
            {

            }

            return this.View(this.ViewName);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public ActionResult Delete(T model, string returnUrl)
        {
            if (model != null && !string.IsNullOrWhiteSpace(model.ScoId) && this.LoginWithSession().Success)
            {
                this.AdobeConnect.DeleteSco(model.ScoId);
            }

            return this.RedirectToLocal(returnUrl);
        }

        #endregion
    }
}
