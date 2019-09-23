############## CLEAN ##################
drop table PatientCarbohydrates_MealItem;
drop table MealItem;
drop table PatientBloodSugar;
drop table PatientExercise;
drop table PatientCarbohydrates;
drop table Patient;
drop table Doctor;
drop table User;

############## INITIALIZE ##################

# Create Table User
create table User
(
	UserID int auto_increment,
	Password varchar(255) not null,
	FirstName varchar(150) not null,
	MiddleName varchar(150) not null,
	LastName varchar(150) not null,
	Email varchar(255) null,
	PhoneNumber varchar(11) null,
	constraint User_pk
		primary key (UserID)
);

# Create Table Patient
create table Patient
(
	PatientID int not null,
	DoctorID int not null,
	constraint Patient_pk
		primary key (PatientID)
);

# Create Table Doctor
create table Doctor
(
	DoctorID int not null,
	NumberOfPatients int not null,
	constraint Doctor_pk
		primary key (DoctorID)
);

# Create Table PatientCarbohydrates
create table PatientCarbohydrates
(
	PatientID int not null,
	TotalCarbs int not null,
	MealName varchar(255) not null,
	FoodCarbs int not null,
	Meal varchar(100) not null,
	TimeOfDay DATETIME null,
	constraint PatientCarbohydrates_pk
		primary key (PatientID)
);

# Create Table PatientExercise
create table PatientExercise
(
	PatientID int not null,
	HoursExercised int not null,
	TimeOfDay DATETIME null,
	constraint PatientExercise_pk
		primary key (PatientID)
);

# Create Table PatientBloodSugar
create table PatientBloodSugar
(
	PatientID int not null,
	LevelBefore FLOAT not null,
	LevelAfter FLOAT not null,
	Meal varchar(100) not null,
	TimeOfDay DateTime null,
	constraint PatientBloodSugar_pk
		primary key (PatientID)
);

# Create Table PatientCarbohydrates_MealItem
create table PatientCarbohydrates_MealItem
(
	PatientID int not null,
	FoodID int not null
);

# Create Table MealItem
create table MealItem
(
	FoodID int auto_increment,
	FoodName varchar(255) not null,
	Carbs int not null,
	constraint MealItem_pk
		primary key (FoodID)
);

############## CONSTRAIN ##################
# Add Constraints to Patient
alter table Patient add constraint PatientID_fk foreign key (PatientID) references User (UserID);
alter table Patient add constraint PatientDoctorID_fk foreign key (DoctorID) references Doctor (DoctorID);

# Add Constraints to Doctor
alter table Doctor add constraint DoctorID_fk foreign key (DoctorID) references User (UserID);

# Add Constraints to PatientCarbohydrates
alter table PatientCarbohydrates add constraint PatientCarbohydrates_Patient_PatientID_fk foreign key (PatientID) references Patient (PatientID);

# Add Constraints to PatientCarbohydrates_MealItem
alter table PatientCarbohydrates_MealItem add constraint MealItemID_fk foreign key (FoodID) references MealItem (FoodID);
alter table PatientCarbohydrates_MealItem add constraint PatientCarbohydratesID_fk foreign key (PatientID) references PatientCarbohydrates (PatientID);

# Add Constraints to PatientExercise
alter table PatientExercise add constraint PatientExerciseID_fk foreign key (PatientID) references Patient (PatientID);

# Add Constraints to PatientBloodSugar
alter table PatientBloodSugar add constraint PatientBloodSugarID_fk foreign key (PatientID) references Patient (PatientID);