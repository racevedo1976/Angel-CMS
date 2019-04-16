


select f.Id as folder_id, f.OwnerId, f.OwnerLevel, f.Title as folder_title, fd.DocumentId, fd.FileName , fd.FileType
from cms.FileDocument fd
inner join cms.FolderItem fi on fd.DocumentId = fi.DocumentId
inner join cms.Folder f on fi.FolderId = f.Id
where fd.FileType = 'film'
order by f.OwnerId, f.Title, fd.FileName

