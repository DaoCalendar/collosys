// -- FILE ------------------------------------------------------------------
// name       : TimeLineMomentCollection.cs
// project    : Itenso Time Period
// created    : Jani Giannoudis - 2011.03.31
// language   : C# 4.0
// environment: .NET 2.0
// copyright  : (c) 2011-2012 by Itenso GmbH, Switzerland
// --------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;

namespace Itenso.TimePeriod
{

	// ------------------------------------------------------------------------
	public class TimeLineMomentCollection : ITimeLineMomentCollection
	{

		// ----------------------------------------------------------------------
		public TimeLineMomentCollection()
		{
		} // TimeLineMomentCollection

		// ----------------------------------------------------------------------
		public TimeLineMomentCollection( IEnumerable<ITimePeriod> periods )
		{
			AddAll( periods );
		} // TimeLineMomentCollection

		// ----------------------------------------------------------------------
		public int Count
		{
			get { return timeLineMoments.Count; }
		} // Count

		// ----------------------------------------------------------------------
		public bool IsEmpty
		{
			get { return Count == 0; }
		} // IsEmpty

		// ----------------------------------------------------------------------
		public ITimeLineMoment Min
		{
			get { return !IsEmpty ? timeLineMoments[ 0 ] : null; }
		} // Min

		// ----------------------------------------------------------------------
		public ITimeLineMoment Max
		{
			get { return !IsEmpty ? timeLineMoments[ Count - 1 ] : null; }
		} // Max

		// ----------------------------------------------------------------------
		public ITimeLineMoment this[ int index ]
		{
			get { return timeLineMoments[ index ]; }
		} // this[]

		// ----------------------------------------------------------------------
		protected IList<ITimeLineMoment> Moments
		{
			get { return timeLineMoments; }
		} // Moments

		// ----------------------------------------------------------------------
		public void Clear()
		{
			timeLineMoments.Clear();
		} // Clear

		// ----------------------------------------------------------------------
		public void Add( ITimePeriod period )
		{
			if ( period == null )
			{
				throw new ArgumentNullException( "period" );
			}
			AddPeriod( period.Start, period );
			AddPeriod( period.End, period );
			Sort();
		} // Add

		// ----------------------------------------------------------------------
		public void AddAll( IEnumerable<ITimePeriod> periods )
		{
			if ( periods == null )
			{
				throw new ArgumentNullException( "periods" );
			}

			foreach ( ITimePeriod period in periods )
			{
				AddPeriod( period.Start, period );
				AddPeriod( period.End, period );
			}
			Sort();
		} // AddAll

		// ----------------------------------------------------------------------
		public void Remove( ITimePeriod period )
		{
			if ( period == null )
			{
				throw new ArgumentNullException( "period" );
			}

			RemovePeriod( period.Start, period );
			RemovePeriod( period.End, period );
			Sort();
		} // Remove

		// ----------------------------------------------------------------------
		public ITimeLineMoment Find( DateTime moment )
		{
			foreach ( ITimeLineMoment timeLineMoment in timeLineMoments )
			{
				if ( timeLineMoment.Moment.Equals( moment ) )
				{
					return timeLineMoment;
				}
			}
			return null;
		} // Find

		// ----------------------------------------------------------------------
		public bool Contains( DateTime moment )
		{
			return Find( moment ) != null;
		} // Contains

		// ----------------------------------------------------------------------
		public bool HasOverlaps()
		{
			bool hasOverlap = false;
			if ( Count > 1 )
			{
				bool start = true;
				foreach ( ITimeLineMoment timeLineMoment in timeLineMoments )
				{
					if ( start )
					{
						if ( timeLineMoment.StartCount != 1 || timeLineMoment.EndCount > 1 )
						{
							hasOverlap = true;
							break;
						}
					}
					else // end
					{
						if ( timeLineMoment.StartCount > 1 || timeLineMoment.EndCount != 1 )
						{
							hasOverlap = true;
							break;
						}
					}
					start = ( timeLineMoment.EndCount - timeLineMoment.StartCount ) > 0;
				}
			}

			return hasOverlap;
		} // HasOverlaps

		// ----------------------------------------------------------------------
		public bool HasGaps()
		{
			bool hasGaps = false;
			if ( Count > 1 )
			{
				int momentCount = 0;
				for ( int index = 0; index < timeLineMoments.Count; index++ )
				{
					ITimeLineMoment timeLineMoment = timeLineMoments[ index ];
					momentCount += timeLineMoment.StartCount;
					momentCount -= timeLineMoment.EndCount;
					if ( momentCount == 0 && index > 0 && index < timeLineMoments.Count - 1 )
					{
						hasGaps = true;
						break;
					}
				}
			}

			return hasGaps;
		} // HasGaps

		// ----------------------------------------------------------------------
		public IEnumerator<ITimeLineMoment> GetEnumerator()
		{
			return timeLineMoments.GetEnumerator();
		} // GetEnumerator

		// ----------------------------------------------------------------------
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		} // IEnumerable.GetEnumerator

		// ----------------------------------------------------------------------
		protected virtual void AddPeriod( DateTime moment, ITimePeriod period )
		{
			ITimeLineMoment timeLineMoment = Find( moment );
			if ( timeLineMoment == null )
			{
				timeLineMoment = new TimeLineMoment( moment );
				timeLineMoments.Add( timeLineMoment );
			}
			timeLineMoment.Periods.Add( period );
		} // AddPeriod

		// ----------------------------------------------------------------------
		protected virtual void RemovePeriod( DateTime moment, ITimePeriod period )
		{
			ITimeLineMoment timeLineMoment = Find( moment );
			if ( timeLineMoment == null || !timeLineMoment.Periods.Contains( period ) )
			{
				throw new InvalidOperationException();
			}

			timeLineMoment.Periods.Remove( period );
			if ( timeLineMoment.Periods.Count == 0 )
			{
				timeLineMoments.Remove( timeLineMoment );
			}
		} // RemovePeriod

		// ----------------------------------------------------------------------
		protected virtual void Sort()
		{
			timeLineMoments.Sort( ( left, right ) => left.Moment.CompareTo( right.Moment ) );
		} // Sort

		// ----------------------------------------------------------------------
		// members
		private readonly List<ITimeLineMoment> timeLineMoments = new List<ITimeLineMoment>();

	} // class TimeLineMomentCollection

} // namespace Itenso.TimePeriod
// -- EOF -------------------------------------------------------------------
