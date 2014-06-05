using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using NHibernate;
using NHibernate.Criterion;
using ColloSys.UserInterface.Areas.Generic.ViewModels;

namespace ColloSys.UserInterface.Areas.Stakeholder2.Models
{
    public class WorkingModel
    {
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

        private IList<string> GetListForValues(WorkingModel workingModel)
        {
            var query = CreateQuery(workingModel);
            return query.Select(Projections.Distinct(Projections.Property(QueryFor.ToString()))).List<string>();
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
                query.And(x => x.District == workingModel.SelectedPincodeData.District);

            if (workingModel.DisplayManager.ShowArea && !string.IsNullOrEmpty(workingModel.SelectedPincodeData.Area))
                query.And(x => x.Area == workingModel.SelectedPincodeData.Area);
            return query;
        }

        public WorkingModel SetWorkingList(WorkingModel workingModel)
        {
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

        public WorkingModel GetGPincodeData(WorkingModel workingModel)
        {
            if (workingModel.MultiSelectValues.Count == 0)
                return workingModel;
            var query = CreateQuery(workingModel);
            workingModel.GPincodes = query.Where(Restrictions.In(workingModel.QueryFor.ToString(), workingModel.MultiSelectValues)).List<GPincode>();

            return workingModel;
        }
    }

    public class DisplayManager
    {
        public DisplayManager()
        {
            ShowCountry = true;
            ShowRegion = false;
            ShowState = false;
            ShowDistrict = false;
            ShowCity = false;
            ShowCluster = false;
            ShowArea = false;
        }
        public bool ShowCountry { get; set; }
        public bool ShowRegion { get; set; }
        public bool ShowState { get; set; }
        public bool ShowDistrict { get; set; }
        public bool ShowCluster { get; set; }
        public bool ShowCity { get; set; }
        public bool ShowArea { get; set; }
    }

    public enum LocationLevels
    {
        Country,
        Region,
        State,
        Cluster,
        District,
        City,
        Area
    }
}