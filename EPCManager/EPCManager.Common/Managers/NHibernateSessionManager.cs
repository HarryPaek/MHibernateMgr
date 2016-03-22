using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using System;


namespace EPCManager.Common.Managers
{
    public sealed class NHibernateSessionManager
    {
        private static ISessionFactory SessionFactory { get; set; }
        private static ISession Session { get; set; }

        static NHibernateSessionManager()
        {
            var configuration = new Configuration();
            configuration.AddAssembly("EPCManager.Repositories");
            SessionFactory = configuration.BuildSessionFactory();
        }

        /// <summary>
        /// Gets the current session.
        /// </summary>
        public static ISession GetCurrentSession()
        {
            if (Session != null)
                return Session;

            Session = SessionFactory.OpenSession();

            return Session;
        }


        /// <summary>
        /// Closes the session.
        /// </summary>
        public static void CloseSession()
        {
            if (Session != null && Session.IsOpen)
                Session.Close();

            Session.Dispose();
            Session = null;
        }


        /// <summary>
        /// Commits the session.
        /// </summary>
        /// <param name="session">The session.</param>
        public static void CommitSession(ISession session)
        {
            try
            {
                session.Transaction.Commit();
            }
            catch (Exception)
            {
                session.Transaction.Rollback();
                throw;
            }
        }

    }
}
