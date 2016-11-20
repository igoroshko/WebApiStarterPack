/*
 * Copyright (c) 2016, TopCoder, Inc. All rights reserved.
 */
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using $safeprojectname$.Entities;

namespace $safeprojectname$.Services.Impl
{
    /// <summary>
    /// This class provides access to the database.
    /// </summary>
    /// 
    /// <threadsafety>
    /// This class is mutable but effectively thread-safe.
    /// </threadsafety>
    ///
    /// <author>TCSCODER</author>
    /// <version>1.0</version>
    /// <copyright>Copyright (c) 2016, TopCoder, Inc. All rights reserved.</copyright>
    public class CustomDbContext : DbContext 
    {
        /// <summary>
        /// Gets or sets the action permission set.
        /// </summary>
        /// <value>
        /// The action permission set.
        /// </value>
        public DbSet<ActionPermission> ActionPermissionSet { get; set; }

        /// <summary>
        /// Gets or sets the token set.
        /// </summary>
        /// <value>
        /// The token set.
        /// </value>
        public DbSet<Token> TokenSet { get; set; }

        /// <summary>
        /// Initializes the <see cref="CustomDbContext"/> class.
        /// </summary>
        static CustomDbContext()
        {
            Database.SetInitializer<CustomDbContext>(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDbContext"/> class.
        /// </summary>
        /// <param name="connectionStringName">The name of the connection string to use.</param>
        public CustomDbContext(string connectionStringName)
            : base(connectionStringName)
        {
        }

        /// <summary>
        /// Customizes mappings between entity model and database.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
