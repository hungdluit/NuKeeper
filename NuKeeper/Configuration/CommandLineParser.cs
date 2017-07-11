﻿using System;
using System.Linq;
using EasyConfig;

namespace NuKeeper.Configuration
{
    public static class CommandLineParser
    {
        public static Settings ReadSettings(string[] args)
        {
            var settings = Config.Populate<CommandLineArguments>(args);
            
            Console.WriteLine($"Running NuKeeper in {settings.Mode} mode");

            switch (settings.Mode)
            {
                case Settings.RepositoryMode:
                    return new Settings(ReadSettingsForRepositoryMode(settings));
                case Settings.OrganisationMode:
                    return new Settings(ReadSettingsForOrganisationMode(settings));
                default:
                    Console.WriteLine($"Mode {settings.Mode} not supported");
                    return null;
            }
        }

        private static RepositoryModeSettings ReadSettingsForRepositoryMode(CommandLineArguments settings)
        {
            var githubToken = settings.GithubToken;
            var githubRepoUri = settings.GithubRepositoryUri;

            // general pattern is https://github.com/owner/reponame.git
            var githubHost = "https://api." + githubRepoUri.Host;
            var path = githubRepoUri.AbsolutePath;
            var pathParts = path.Split('/')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            var repoOwner = pathParts[0];
            var repoName = pathParts[1].Replace(".git", string.Empty);

            return new RepositoryModeSettings
            {
                GithubUri = githubRepoUri,
                GithubToken = githubToken,
                GithubApiBase = new Uri(githubHost),
                RepositoryName = repoName,
                RepositoryOwner = repoOwner
            };
        }

        private static OrganisationModeSettings ReadSettingsForOrganisationMode(CommandLineArguments settings)
        {
            var githubToken = settings.GithubToken;
            var githubHost = settings.GithubApiEndpoint;
            var githubOrganisationName = settings.GithubOrganisationName;

            return new OrganisationModeSettings
            {
                GithubApiBase = githubHost,
                GithubToken = githubToken,
                OrganisationName = githubOrganisationName
            };
        }
    }
}
