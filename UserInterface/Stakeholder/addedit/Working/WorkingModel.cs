#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.SessionMgr;
using NHibernate;
using NHibernate.Criterion;
using ColloSys.UserInterface.Areas.Generic.ViewModels;

#endregion

namespace AngularUI.Stakeholder.addedit.Working
{
    public class WorkingModel
    {
        #region ctor
        public WorkingModel()
        {
            DisplayManager = new DisplayManager();
            SelectedPincodeData = new PincodeData();
            ListOfAreas = new List<string>();
            ListOfCities = new List<string>();
            ListOfClusters = new List<string>();
            ListOfDistricts = new List<string>();
            ListOfRegions = new List<string>();
            ListOfStates = new List<string>();
            MultiSelectValues = new List<string>();
        }

        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public DisplayManager DisplayManager { get; set; }
        public PincodeData SelectedPincodeData { get; set; }
        public IList<string> ListOfRegions { get; set; }
        public IList<string> ListOfStates { get; set; }
        public IList<string> ListOfClusters { get; set; }
        public IList<string> ListOfDistricts { get; set; }
        public IList<string> ListOfCities { get; set; }
        public IList<string> ListOfAreas { get; set; }
        public LocationLevels QueryFor { get; set; }
        public IList<GPincode> GPincodes { get; set; }
        public List<string> MultiSelectValues { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global
        #endregion

        #region get value list
        private static void ResetSelectedPincode(WorkingModel workingModel, LocationLevels queryFor)
        {
            switch (queryFor)
            {
                case LocationLevels.Country:
                    break;
                case LocationLevels.Region:
                    workingModel.SelectedPincodeData.Region = null;
                    workingModel.ListOfRegions = new List<string>();
                    goto case LocationLevels.State;
                case LocationLevels.State:
                    workingModel.SelectedPincodeData.State = null;
                    workingModel.ListOfStates = new List<string>();
                    goto case LocationLevels.Cluster;
                case LocationLevels.Cluster:
                    workingModel.SelectedPincodeData.Cluster = null;
                    workingModel.ListOfClusters = new List<string>();
                    goto case LocationLevels.District;
                case LocationLevels.District:
                    workingModel.SelectedPincodeData.District = null;
                    workingModel.ListOfDistricts = new List<string>();
                    goto case LocationLevels.City;
                case LocationLevels.City:
                    workingModel.SelectedPincodeData.City = null;
                    workingModel.ListOfCities = new List<string>();
                    goto case LocationLevels.Area;
                case LocationLevels.Area:
                    workingModel.SelectedPincodeData.Area = null;
                    workingModel.ListOfAreas = new List<string>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IList<string> GetListForValues(WorkingModel workingModel)
        {
            var query = CreateQuery(workingModel);
            return query.Select(
                Projections.Distinct(Projections.Property(QueryFor.ToString())))
                .List<string>();
        }

        private static IQueryOver<GPincode, GPincode> CreateQuery(WorkingModel workingModel)
        {
            var session = SessionManager.GetCurrentSession();
            var query = session.QueryOver<GPincode>()
                               .Where(x => x.Country == "India" && x.IsInUse);

            if (workingModel.DisplayManager.ShowRegion && !string.IsNullOrEmpty(workingModel.SelectedPincodeData.Region))
                query.And(x => x.Region == workingModel.SelectedPincodeData.Region);

            if (workingModel.DisplayManager.ShowState && !string.IsNullOrEmpty(workingModel.SelectedPincodeData.State))
                query.And(x => x.State == workingModel.SelectedPincodeData.State);

            if (workingModel.DisplayManager.ShowCluster && !string.IsNullOrEmpty(workingModel.SelectedPincodeData.Cluster))
                query.And(x => x.Cluster == workingModel.SelectedPincodeData.Cluster);

            if (workingModel.DisplayManager.ShowDistrict && !string.IsNullOrEmpty(workingModel.SelectedPincodeData.District))
                query.And(x => x.District == workingModel.SelectedPincodeData.District);

            if (workingModel.DisplayManager.ShowCity && !string.IsNullOrEmpty(workingModel.SelectedPincodeData.City))
                query.And(x => x.City == workingModel.SelectedPincodeData.City);

            if (workingModel.DisplayManager.ShowArea && !string.IsNullOrEmpty(workingModel.SelectedPincodeData.Area))
                query.And(x => x.Area == workingModel.SelectedPincodeData.Area);

            return query;
        }

        public WorkingModel SetWorkingList(WorkingModel workingModel)
        {
            ResetSelectedPincode(workingModel, QueryFor);

            switch (QueryFor)
            {
                case LocationLevels.Country:
                    break;
                case LocationLevels.Region:
                    workingModel.ListOfRegions = GetListForValues(workingModel);
                    break;
                case LocationLevels.State:
                    workingModel.ListOfStates = GetListForValues(workingModel);
                    break;
                case LocationLevels.Cluster:
                    workingModel.ListOfClusters = GetListForValues(workingModel);
                    break;
                case LocationLevels.District:
                    workingModel.ListOfDistricts = GetListForValues(workingModel);
                    break;
                case LocationLevels.City:
                    workingModel.ListOfCities = GetListForValues(workingModel);
                    break;
                case LocationLevels.Area:
                    workingModel.ListOfAreas = GetListForValues(workingModel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return workingModel;
        }
        #endregion

        #region get pincodes
        public WorkingModel GetGPincodeData(WorkingModel workingModel)
        {
            if (workingModel.MultiSelectValues.Count == 0)
                return workingModel;
            var query = CreateQuery(workingModel);
            workingModel.GPincodes = query.Where(Restrictions.In(workingModel.QueryFor.ToString(), workingModel.MultiSelectValues)).List<GPincode>();

            return workingModel;
        }
        #endregion
    }
}