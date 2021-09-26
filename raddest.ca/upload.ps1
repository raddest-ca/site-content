$config = Get-Content .\config.ini | ConvertFrom-StringData;

Write-Host "Uploading content";
az storage blob upload-batch `
    --account-name $config.storageAccount `
    --source .\content `
    --destination '$web' `


Write-Host "Busting cache";
# https://www.davepaquette.com/archive/2020/05/10/deploying-a-static-site-to-azure-using-the-az-cli.aspx
az cdn endpoint purge `
    --resource-group $config.cdnResourceGroup `
    --name $config.cdnEndpoint `
    --profile-name $config.cdnProfile `
    --no-wait `
    --content-paths '/' '/index.html';