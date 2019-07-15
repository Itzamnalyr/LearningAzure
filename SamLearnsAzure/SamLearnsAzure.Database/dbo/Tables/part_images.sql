CREATE TABLE [dbo].[part_images]
(
	[part_image_id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[part_num] VARCHAR(100) NOT NULL,
	[source_image_url] VARCHAR(2000) NULL,
	[color_id] INT NOT NULL,
	[last_updated] DATETIME NOT NULL
    CONSTRAINT [FK_part_images_parts] FOREIGN KEY ([part_num]) REFERENCES [parts]([part_num]),
    CONSTRAINT [FK_part_images_colors] FOREIGN KEY ([color_id]) REFERENCES [colors]([id]), 
)
