//
// BSATroop53 Files Website DevOps - Build Tools.
// Copyright (C) 2024 Seth Hendrick
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Core.IO;
using Cake.Frosting;

namespace DevOps.Tasks
{
    [TaskName( "build_metrics" )]
    public sealed class BuildMetricsTask : FrostingTask<BuildContext>
    {
        public override void Run( BuildContext context )
        {
            FilePath outputPath = context.PublicDir.CombineWithFilePath(
                new FilePath( "metrics.txt" )
            );

            if( context.FileExists( outputPath ) )
            {
                context.DeleteFile( outputPath );
            }

            IEnumerable<string> files = Directory.EnumerateFiles(
                context.PublicDir.FullPath,
                "*.*",
                SearchOption.AllDirectories
            );

            long totalFiles = files.LongCount();
            long totalFileSize = files.Sum( f => new FileInfo( f ).Length );

            string metricsString =
@$"
# HELP files_bsatroop53_public_files_size_total Total file size of everything in the public folder in bytes.
# TYPE files_bsatroop53_public_files_size_total gauge
files_bsatroop53_public_files_size_total {totalFileSize}
# HELP files_bsatroop53_public_files_total Total number of files in the public folder.
# TYPE files_bsatroop53_public_files_total gauge
files_bsatroop53_public_files_total {totalFiles}
";
            File.WriteAllText( outputPath.FullPath, metricsString );
            context.Information( "Wrote out metrics.txt successfully!" );
        }
    }
}
