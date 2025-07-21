using GitHubSearchApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubSearchApp.Application.Utils
{
    public static class RelevanceCalculator
    {
        public static int Calcular(Repository repo)
        {
            return repo.Stars * 2 + repo.Forks + repo.Watchers;
        }
    }
}
