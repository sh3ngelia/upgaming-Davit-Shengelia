create database BookCatalog;
go

use BookCatalog;
go

create table Authors
(
	AuthorID int identity(1,1) primary key
	,[Name] nvarchar(100) not null
)

create table Books
(
	ID int identity(1,1) primary key
	,Title nvarchar(200) not null
	,AuthorID int not null
	,PublicationYear int not null
	,foreign key (AuthorID) references Authors(AuthorID)
)


insert into Authors values
('Dato Turashvili'),
('Shota Rustaveli');

insert into Books values
('Jeans Generation', 1, 2008),
('The Knight in the Panther''s Skin', 2, 1186),
('The King of Forests', 1, 2013);


update Books set PublicationYear = 2018 where ID = 3;

delete from Books where ID = 2;


select b.Title, a.Name as AuthorName
from Books b
join Authors a on b.AuthorID = a.AuthorID
where b.PublicationYear >2010;