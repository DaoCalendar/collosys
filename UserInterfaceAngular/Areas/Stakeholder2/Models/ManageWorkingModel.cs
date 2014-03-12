#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.Shared.SharedUtils;

#endregion
//stakeholders calls changed
//hierarchy calls changed
namespace ColloSys.UserInterface.Areas.Stakeholder2.Models
{
    public class ManageWorkingModel
    {
        public Stakeholders Source { get; set; }
        public IList<StakeWorkingVM> Workings { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Reason { get; set; }

        private static readonly StakeQueryBuilder StakeQuery=new StakeQueryBuilder();

        public static IList<Stakeholders> ChangeWorking(ManageWorkingModel manageWorking)
        {
            if (manageWorking.Reason.ToUpper() == "Leave".ToUpper())
            {
                SetLeaveForStakeholder(manageWorking);
            }

            if (manageWorking.Reason.ToUpper() == "Exit".ToUpper())
            {
                SetExitForStakeholder(manageWorking);
            }
            manageWorking.Source = SetReferences(manageWorking.Source);
            StakeQuery.Save(manageWorking.Source);
            foreach (var stakeWorkingVm in manageWorking.Workings)
            {
                stakeWorkingVm.Stakeholders = SetReferences(stakeWorkingVm.Stakeholders);
                StakeQuery.Save(stakeWorkingVm.Stakeholders);
            }
            return null;
        }

        private static void SetExitForStakeholder(ManageWorkingModel manageWorking)
        {
            foreach (var stakeWorkingVm in manageWorking.Workings)
            {
                var newWorking = ReflectionUtil.CloneObject(stakeWorkingVm.Working);

                newWorking = ClearBaseProperties(newWorking);

                stakeWorkingVm.Working.EndDate = manageWorking.StartDate.HasValue
                                                     ? manageWorking.StartDate.Value
                                                     : DateTime.Now;
                foreach (var stkhWorking in manageWorking.Source.StkhWorkings)
                {
                    if (stkhWorking.Id == stakeWorkingVm.Working.Id)
                        stkhWorking.EndDate = manageWorking.StartDate.HasValue
                                                  ? manageWorking.StartDate.Value
                                                  : DateTime.Now;
                }
                newWorking.StartDate = manageWorking.StartDate.HasValue
                                           ? manageWorking.StartDate.Value.AddDays(1)
                                           : DateTime.Now;

                if (manageWorking.Source.Hierarchy.HasPayment)
                {
                    var newPayment = ReflectionUtil.CloneObject(stakeWorkingVm.Working.StkhPayment);

                    newPayment = ClearBaseProperties(newPayment);

                    stakeWorkingVm.Working.StkhPayment.EndDate = manageWorking.StartDate.HasValue
                                                                     ? manageWorking.StartDate.Value
                                                                     : DateTime.Now;
                    foreach (var stkhPayment in manageWorking.Source.StkhPayments)
                    {
                        if (stkhPayment.Id == stakeWorkingVm.Working.StkhPayment.Id)
                            stkhPayment.EndDate = manageWorking.StartDate.HasValue
                                                      ? manageWorking.StartDate.Value
                                                      : DateTime.Now;
                    }

                    newPayment.StartDate = manageWorking.StartDate.HasValue
                                               ? manageWorking.StartDate.Value.AddDays(1)
                                               : DateTime.Now;
                    newPayment.StkhWorkings.Clear();
                    newPayment.StkhWorkings.Add(newWorking);
                    newWorking.StkhPayment = newPayment;

                    stakeWorkingVm.Stakeholders.StkhPayments.Add(newPayment);
                }
                stakeWorkingVm.Stakeholders.StkhWorkings.Add(newWorking);
            }

        }

        private static void SetLeaveForStakeholder(ManageWorkingModel manageWorking)
        {
            foreach (var stakeWorkingVm in manageWorking.Workings)
            {
                var newWorking = ReflectionUtil.CloneObject(stakeWorkingVm.Working);
                var oldStakeNewWorking = ReflectionUtil.CloneObject(stakeWorkingVm.Working);

                newWorking = ClearBaseProperties(newWorking);
                oldStakeNewWorking = ClearBaseProperties(oldStakeNewWorking);

                stakeWorkingVm.Working.EndDate = manageWorking.StartDate.HasValue
                                                     ? manageWorking.StartDate.Value
                                                     : DateTime.Now;
                foreach (var stkhWorking in manageWorking.Source.StkhWorkings)
                {
                    if (stkhWorking.Id == stakeWorkingVm.Working.Id)
                        stkhWorking.EndDate = manageWorking.StartDate.HasValue
                                                  ? manageWorking.StartDate.Value
                                                  : DateTime.Now;
                }

                newWorking.StartDate = manageWorking.StartDate.HasValue
                                           ? manageWorking.StartDate.Value.AddDays(1)
                                           : DateTime.Now;
                newWorking.EndDate = manageWorking.EndDate.HasValue
                                         ? manageWorking.EndDate.Value
                                         : DateTime.Now;


                oldStakeNewWorking.StartDate = manageWorking.EndDate.HasValue
                                                   ? manageWorking.EndDate.Value.AddDays(1)
                                                   : DateTime.Now;

                if (manageWorking.Source.Hierarchy.HasPayment)
                {
                    var newPayment = ReflectionUtil.CloneObject(stakeWorkingVm.Working.StkhPayment);
                    var oldStakePayment = ReflectionUtil.CloneObject(stakeWorkingVm.Working.StkhPayment);

                    newPayment = ClearBaseProperties(newPayment);
                    oldStakePayment = ClearBaseProperties(oldStakePayment);

                    stakeWorkingVm.Working.StkhPayment.EndDate = manageWorking.StartDate.HasValue
                                                                     ? manageWorking.StartDate.Value
                                                                     : DateTime.Now;

                    foreach (var stkhPayment in manageWorking.Source.StkhPayments)
                    {
                        if (stkhPayment.Id == stakeWorkingVm.Working.StkhPayment.Id)
                            stkhPayment.EndDate = manageWorking.StartDate.HasValue
                                                      ? manageWorking.StartDate.Value
                                                      : DateTime.Now;
                    }

                    newPayment.StartDate = manageWorking.StartDate.HasValue
                                               ? manageWorking.StartDate.Value.AddDays(1)
                                               : DateTime.Now;
                    newPayment.EndDate = manageWorking.EndDate.HasValue
                                             ? manageWorking.EndDate.Value
                                             : DateTime.Now;

                    oldStakePayment.StartDate = manageWorking.EndDate.HasValue
                                                    ? manageWorking.EndDate.Value.AddDays(1)
                                                    : DateTime.Now;
                    oldStakeNewWorking.StkhPayment = oldStakePayment;
                    oldStakePayment.StkhWorkings.Add(oldStakeNewWorking);

                    newPayment.StkhWorkings.Clear();
                    newPayment.StkhWorkings.Add(newWorking);
                    newWorking.StkhPayment = newPayment;

                    manageWorking.Source.StkhPayments.Add(oldStakePayment);
                    stakeWorkingVm.Stakeholders.StkhPayments.Add(newPayment);
                }
                stakeWorkingVm.Stakeholders.StkhWorkings.Add(newWorking);
                manageWorking.Source.StkhWorkings.Add(oldStakeNewWorking);
            }

        }


        public static T ClearBaseProperties<T>(T entity) where T : Entity
        {
            entity.Id = Guid.Empty;
            entity.Version = 0;
            return entity;
        }

        private static Stakeholders SetReferences(Stakeholders stakeholders)
        {
            foreach (var stkhWorking in stakeholders.StkhWorkings)
            {
                stkhWorking.Stakeholder = stakeholders;
            }
            foreach (var stkhPayment in stakeholders.StkhPayments)
            {
                stkhPayment.Stakeholder = stakeholders;
            }
            foreach (var stakeAddress in stakeholders.GAddress)
            {
                stakeAddress.Stakeholder = stakeholders;
            }
            foreach (var stkhRegistration in stakeholders.StkhRegistrations)
            {
                stkhRegistration.Stakeholder = stakeholders;
            }
            return stakeholders;
        }
    }

    public class StakeWorkingVM
    {
        public Stakeholders Stakeholders { get; set; }
        public StkhWorking Working { get; set; }
    }
}


//public static object CloneObject(object objSource)
//{
//    //step : 1 Get the type of source object and create a new instance of that type
//    Type typeSource = objSource.GetType();
//    object objTarget = Activator.CreateInstance(typeSource);

//    //Step2 : Get all the properties of source object type
//    PropertyInfo[] propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

//    //Step : 3 Assign all source property to taget object 's properties
//    foreach (PropertyInfo property in propertyInfo)
//    {
//        //Check whether property can be written to
//        if (property.CanWrite)
//        {
//            //Step : 4 check whether property type is value type, enum or string type
//            if (property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType.Equals(typeof(System.String)))
//            {
//                property.SetValue(objTarget, property.GetValue(objSource, null), null);
//            }
//            //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
//            //else
//            //{
//            //    object objPropertyValue = property.GetValue(objSource, null);
//            //    if (objPropertyValue == null)
//            //    {
//            //        property.SetValue(objTarget, null, null);
//            //    }
//            //    else
//            //    {
//            //        property.SetValue(objTarget, CloneObject(objPropertyValue), null);
//            //    }
//            //}
//        }
//    }
//    return objTarget;
//}