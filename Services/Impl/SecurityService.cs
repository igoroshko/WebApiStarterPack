/*
 * Copyright (c) 2016, TopCoder, Inc. All rights reserved.
 */
using System;
using Microsoft.Practices.Unity;
using $safeprojectname$.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using $safeprojectname$.Exceptions;
using System.Web.Services.Protocols;

namespace $safeprojectname$.Services.Impl
{
    /// <summary>
    /// This service class provides operations for managing security operations.
    /// </summary>
    /// 
    /// <threadsafety>
    /// This class is mutable but effectively thread-safe.
    /// </threadsafety>
    ///
    /// <author>TCSCODER</author>
    /// <version>1.0</version>
    /// <copyright>Copyright (c) 2016, TopCoder, Inc. All rights reserved.</copyright>
    public class SecurityService : BasePersistenceService, ISecurityService
    {
        /// <summary>
        /// Gets or sets the token expiration time in minutes.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// It is expected to be initialized by Unity and never changed after that.
        /// Should be positive after initialization.
        /// </para>
        /// It is used for managing token expirations.
        /// </remarks>
        ///
        /// <value>The token expiration time in minutes.</value>
        [Dependency]
        public int TokenExpiration { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityService"/> class.
        /// </summary>
        public SecurityService()
        {
        }

        /// <summary>
        /// Checks that all configuration properties were properly initialized.
        /// </summary>
        ///
        /// <exception cref="ConfigurationException">
        /// If any of required injection fields are not injected or have invalid values.
        /// </exception>
        public override void CheckConfiguration()
        {
            base.CheckConfiguration();

            Helper.ValidateConfigPropertyPositive(TokenExpiration, nameof(TokenExpiration));
        }

        /// <summary>
        /// Authenticates user and returns populated User instance in case of success.
        /// </summary>
        ///
        /// <param name="username">The username.</param>
        /// <param name="password">The user password.</param>
        /// <returns>The authenticated User entity, or <c>null</c> if cannot authenticate user.</returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="username"/> or <paramref name="password"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="username"/> or <paramref name="password"/> is empty.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        public User Authenticate(string username, string password)
        {
            return Logger.Process(() =>
            {
                Helper.ValidateArgumentNotNullOrEmpty(username, nameof(username));
                Helper.ValidateArgumentNotNullOrEmpty(password, nameof(password));

                var user = new User { Username = username };
                return user;
            },
            $"authenticating user '{username}'",
            parameters: new object[] { username, Helper.PasswordMask });
        }

        /// <summary>
        /// Checks whether non-admin user is authorized to perform specified action.
        /// </summary>
        ///
        /// <param name="action">The action to perform.</param>
        /// <returns>True if user is authorized to perform specified action; otherwise False.</returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="action"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="action"/> is empty.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        public bool IsAuthorized(string action)
        {
            return ProcessWithDb(db =>
            {
                Helper.ValidateArgumentNotNullOrEmpty(action, nameof(action));

                return GetDbSet<ActionPermission>(db).Any(x => x.Action == action);
            },
            "checking whether user is authorized to perform specified action",
            parameters: action);
        }

        /// <summary>
        /// Generates token for the given user.
        /// </summary>
        ///
        /// <param name="user">The user.</param>
        /// <returns>The generated token.</returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="user"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        public Token GenerateToken(User user)
        {
            return ProcessWithDb(db =>
            {
                Helper.ValidateArgumentNotNull(user, nameof(user));

                var token = new Token
                {
                    Username = user.Username,
                    TokenValue = Guid.NewGuid().ToString(),
                    ExpirationDate = DateTime.Now.AddMinutes(TokenExpiration)
                };

                GetDbSet<Token>(db).Add(token);
                return token;
            },
            "generating token",
            saveChanges: true,
            parameters: user);
        }

        /// <summary>
        /// Authenticates user and returns populated User instance in case of success.
        /// </summary>
        ///
        /// <param name="token">The token.</param>
        /// <returns>The authenticated User entity, or null if cannot authenticate user.</returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="token"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="token"/> is empty.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        public User Authenticate(string token)
        {
            return ProcessWithDb(db =>
            {
                Helper.ValidateArgumentNotNullOrEmpty(token, nameof(token));

                // get valid token
                DbSet<Token> tokens = GetDbSet<Token>(db);
                Token tk = GetValidToken(token, tokens);
                if (tk == null)
                {
                    return null;
                }

                return new User
                {
                    Username = tk.Username
                };
            },
            "authenticating user with token",
            parameters: token);
        }

        /// <summary>
        /// Revokes the token.
        /// </summary>
        ///
        /// <param name="token">The token to revoke.</param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="token"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="token"/> is empty.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        public void RevokeToken(string token)
        {
            ProcessWithDb(db =>
            {
                Helper.ValidateArgumentNotNullOrEmpty(token, nameof(token));

                DbSet<Token> tokens = GetDbSet<Token>(db);
                Token tk = GetValidToken(token, tokens);
                if (tk != null)
                {
                    tokens.Remove(tk);
                    db.SaveChanges();
                }
            },
            "revoking token",
            parameters: token);
        }

        /// <summary>
        /// Gets the <see cref="Token"/> instance by token value.
        /// </summary>
        /// <param name="token">The token to search for.</param>
        /// <param name="tokens">The database set of tokens.</param>
        /// <returns>Then matching token, or <c>null</c> if not found.</returns>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        private Token GetValidToken(string token, DbSet<Token> tokens)
        {
            return tokens
                .FirstOrDefault(c => c.TokenValue == token && c.ExpirationDate > DateTime.Now);
        }
    }
}
