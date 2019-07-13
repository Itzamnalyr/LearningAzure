CREATE TABLE [dbo].[part_images]
(
	[part_num] VARCHAR(100) NOT NULL PRIMARY KEY,
	[source_image_url] VARCHAR(2000) NULL,
	[new_custom_image_url] VARCHAR(2000) NULL,
	[last_updated] DATETIME NOT NULL
    CONSTRAINT [FK_part_images_parts] FOREIGN KEY ([part_num]) REFERENCES [parts]([part_num]), 
)
