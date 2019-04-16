$hr = "----------------------------------------------------------------------"
write-host 
write-host $hr
write-host " /(  ___________ 	# Angelo Restore Script: [tfs.restore.connect.ps1]  " 
write-host "|  >:===========`	#                                   "
write-host " )(       PC MAC 	#     Brach ID: $env:BUILD_SOURCEBRANCH "
write-host " """"              	#     Build ID: $Env:BUILD_BUILDID      " 
write-host $hr
write-host


$projects = (
	"Angelo.Aegis.Client", 
	"Angelo.Common", 
	"Angelo.Jobs", 
	"Angelo.Jobs.SqlServer", 
	"Angelo.MockData", 
	"Angelo.Connect.Core", 
	"Angelo.Connect.Web" 
)

ForEach ($project in $projects ) {
	
	write-host $hr
	write-host "[Restoring Dependencies for $project]" 
	write-host $hr
	cd $project
	dotnet restore -v "E:\build\packages"
	cd ..
}
