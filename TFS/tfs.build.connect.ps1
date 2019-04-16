$hr = "----------------------------------------------------------------------"
write-host $hr
write-host " /(  ___________ 	# Angelo Build Script: [tfs.build.connect.ps1]  " 
write-host "|  >:===========`	#                                   "
write-host " )(       PC MAC 	#     Brach ID: $env:BUILD_SOURCEBRANCH "
write-host " """"              	#     Build ID: $Env:BUILD_BUILDID      " 


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
	write-host "[Building Project $project]" 
	write-host $hr
	cd $project
	dotnet build 
	cd ..
}