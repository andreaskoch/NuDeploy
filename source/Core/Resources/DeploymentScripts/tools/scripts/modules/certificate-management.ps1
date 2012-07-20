function Import-Certificate
{
	param(
		[string]$FilePath,
		[string]$RootStore = "CurrentUser",
		[string]$CertStore = "My",
		[string]$Password = $null
	)
	
	switch -wildcard ($FilePath)
	{
		"*.pfx"
		{
			Import-PfxCertificate -certPath $FilePath -certRootStore $RootStore -CertStore $CertStore -pfxPass $Password
			break
		}
		
		default
		{
			Import-509Certificate -certPath $FilePath -certRootStore $RootStore -CertStore $CertStore
			break
		}
	}
}

function Import-PfxCertificate
{
	param(
		[String]$certPath,
		[String]$certRootStore = "CurrentUser",
		[String]$certStore = "My",
		$pfxPass = $null
	)
	
	$pfx = new-object System.Security.Cryptography.X509Certificates.X509Certificate2

	if ($pfxPass -eq $null) {
		$pfxPass = read-host "Enter the pfx password" -assecurestring
	}

	$pfx.import($certPath, $pfxPass, "Exportable,PersistKeySet")

	$store = new-object System.Security.Cryptography.X509Certificates.X509Store($certStore,$certRootStore)
	$store.open("MaxAllowed")
	$store.add($pfx)
	$store.close()
}

function Import-509Certificate
{
	param(
		[String]$certPath,
		[String]$certRootStore,
		[String]$certStore
	)

	$pfx = new-object System.Security.Cryptography.X509Certificates.X509Certificate2
	$pfx.import($certPath)

	$store = new-object System.Security.Cryptography.X509Certificates.X509Store($certStore,$certRootStore)
	$store.open("MaxAllowed")
	$store.add($pfx)
	$store.close()
}

Function Remove-Certificate
{ 
	[CmdletBinding()]
	Param(
		[Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$thumbprint,
		
		[Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$certificateRootStore,
		
		[Parameter(Position=2, Mandatory=$True, ValueFromPipeline=$True)]
		[string]$certificateStore  
	)
 
	$currentLocation = (Get-Location)
	
    $certificatePath = "cert:\$certificateRootStore\$certificateStore"    
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store $certificateStore,$certificateRootStore
    $store.Open("ReadWrite")

    set-location $certificatePath
    $certs = @(dir) | where-object {$_.Thumbprint -eq $thumbprint}
    foreach ($cert in $certs)
    {
        if ($cert)
        {
            $store.Remove($cert)
        }
    }
    $store.Close()
	
	Set-Location $currentLocation
}