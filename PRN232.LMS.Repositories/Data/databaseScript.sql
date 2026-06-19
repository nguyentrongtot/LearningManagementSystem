CREATE DATABASE LMS_PRN232;
GO

USE LMS_PRN232;
GO

-- =========================
-- 1. SEMESTER
-- =========================
CREATE TABLE Semester (
    SemesterId INT PRIMARY KEY IDENTITY(1,1),

    SemesterName NVARCHAR(100) NOT NULL,

    StartDate DATETIME NOT NULL,

    EndDate DATETIME NOT NULL
);

-- =========================
-- 2. SUBJECT
-- =========================
CREATE TABLE Subject (
    SubjectId INT PRIMARY KEY IDENTITY(1,1),

    SubjectCode VARCHAR(20) NOT NULL UNIQUE,

    SubjectName NVARCHAR(100) NOT NULL,

    Credit INT NOT NULL
        CHECK (Credit > 0)
);

-- =========================
-- 3. COURSE
-- =========================
CREATE TABLE Course (
    CourseId INT PRIMARY KEY IDENTITY(1,1),

    CourseName NVARCHAR(100) NOT NULL,

    SemesterId INT NOT NULL,

    SubjectId INT NOT NULL,

    CONSTRAINT FK_Course_Semester
        FOREIGN KEY (SemesterId)
        REFERENCES Semester(SemesterId),

    CONSTRAINT FK_Course_Subject
        FOREIGN KEY (SubjectId)
        REFERENCES Subject(SubjectId)
);

-- =========================
-- 4. STUDENT
-- =========================
CREATE TABLE Student (
    StudentId INT PRIMARY KEY IDENTITY(1,1),

    FullName NVARCHAR(100) NOT NULL,

    Email VARCHAR(100) NOT NULL UNIQUE,

    DateOfBirth DATETIME NOT NULL
);

-- =========================
-- 5. ENROLLMENT
-- =========================
CREATE TABLE Enrollment (
    EnrollmentId INT PRIMARY KEY IDENTITY(1,1),

    StudentId INT NOT NULL,

    CourseId INT NOT NULL,

    EnrollDate DATETIME NOT NULL,

    Status VARCHAR(20) NOT NULL
        CHECK (Status IN ('Active', 'Dropped', 'Completed')),

    CONSTRAINT FK_Enrollment_Student
        FOREIGN KEY (StudentId)
        REFERENCES Student(StudentId),

    CONSTRAINT FK_Enrollment_Course
        FOREIGN KEY (CourseId)
        REFERENCES Course(CourseId)
);





INSERT INTO Semester (SemesterName, StartDate, EndDate)
VALUES
('Spring2024', '2024-01-08', '2024-03-26'),
('Summer2024', '2024-05-12', '2024-07-31'),
('Fall2024',   '2024-09-16', '2024-11-25'),

('Spring2025', '2025-01-08', '2025-03-26'),
('Summer2025', '2025-05-12', '2025-07-31'),
('Fall2025',   '2025-09-16', '2025-11-25');



INSERT INTO Subject (SubjectCode, SubjectName, Credit)
VALUES

('PRF192', 'Programming Fundamentals', 3),
('MAE101', 'Mathematics for Engineering', 3),
('CEA201', 'Computer Organization and Architecture', 3),
('SSL101', 'Academic Skills for University Success', 3),
('CSI104', 'Introduction to Computing', 3),

('NWC204', 'Computer Networking', 3),
('SSG104', 'Communication and In-Group Working Skills', 3),
('PRO192', 'Object-Oriented Programming', 3),
('MAD101', 'Discrete Mathematics', 3),
('OSG202', 'Operating Systems', 3),

('CSD201', 'Data Structures and Algorithms', 3),
('DBI202', 'Database Systems', 3),
('LAB211', 'OOP with Java Lab', 3),
('WED201', 'Web Design', 3),
('JPD113', 'Elementary Japanese 1 - A1.1', 3),

('JPD123', 'Elementary Japanese 1 - A1.2', 3),
('PRJ301', 'Java Web Application Development', 3),
('SWE201', 'Introduction to Software Engineering', 3),
('MAS291', 'Statistics & Probability', 3),
('IOT102', 'Internet of Things', 3),

('WDU203', 'The UI/UX Design', 3),
('PRN212', 'Basic Cross-Platform Application Programming With .NET', 3),
('SWR302', 'Software Requirements', 3),
('SWT301', 'Software Testing', 3),
('SWP391', 'Software Development Project', 3),

('OJT202', 'On The Job Training', 10),
('ENW493', 'Research Methods & Academic Writing Skills', 3);






INSERT INTO Student (FullName, Email, DateOfBirth)
VALUES
(N'Nguyễn Văn An',        'nvase180001@fpt.edu.vn', '2004-01-15'),
(N'Trần Đức Bình',        'tdbse180002@fpt.edu.vn', '2004-03-22'),
(N'Lê Minh Cường',        'lmcse180003@fpt.edu.vn', '2003-07-10'),
(N'Phạm Hoàng Dũng',      'phdse180004@fpt.edu.vn', '2004-11-05'),
(N'Nguyễn Thị Em',        'ntese180005@fpt.edu.vn', '2003-09-18'),

(N'Võ Thanh Giang',       'vtgse180006@fpt.edu.vn', '2004-02-27'),
(N'Bùi Quốc Hiếu',        'bqhse180007@fpt.edu.vn', '2003-12-14'),
(N'Đỗ Văn Hùng',          'dvhse180008@fpt.edu.vn', '2004-06-30'),
(N'Trần Minh Khánh',      'tmkse180009@fpt.edu.vn', '2003-04-09'),
(N'Nguyễn Quang Lâm',     'nqlse180010@fpt.edu.vn', '2004-08-21'),

(N'Phạm Gia Long',        'pglse180011@fpt.edu.vn', '2003-10-12'),
(N'Lê Hoàng Minh',        'lhmse180012@fpt.edu.vn', '2004-05-17'),
(N'Trần Đức Nam',         'tdnse180013@fpt.edu.vn', '2003-01-29'),
(N'Nguyễn Văn Phong',     'nvpse180014@fpt.edu.vn', '2004-07-03'),
(N'Lê Minh Quân',         'lmqse180015@fpt.edu.vn', '2003-11-11'),

(N'Đặng Quốc Sơn',        'dqsse180016@fpt.edu.vn', '2004-09-26'),
(N'Nguyễn Anh Thái',      'natse180017@fpt.edu.vn', '2003-06-08'),
(N'Võ Minh Trung',        'vmtse180018@fpt.edu.vn', '2004-04-19'),
(N'Phạm Đức Tuấn',        'pdtse180019@fpt.edu.vn', '2003-08-24'),
(N'Lê Quang Vinh',        'lqvse180020@fpt.edu.vn', '2004-12-02'),

(N'Nguyễn Gia Bảo',       'ngbse180021@fpt.edu.vn', '2004-02-11'),
(N'Trần Nhật Hào',        'tnhse180022@fpt.edu.vn', '2003-05-14'),
(N'Lý Hoàng Khang',       'lhkse180023@fpt.edu.vn', '2004-07-28'),
(N'Phan Minh Luân',       'pmlse180024@fpt.edu.vn', '2003-09-06'),
(N'Đinh Quốc Việt',       'dqvse180025@fpt.edu.vn', '2004-10-30'),

(N'Nguyễn Đức Huy',       'ndhse180026@fpt.edu.vn', '2003-03-19'),
(N'Trương Minh Tài',      'tmtse180027@fpt.edu.vn', '2004-06-17'),
(N'Lê Quốc Khánh',        'lqkse180028@fpt.edu.vn', '2003-12-09'),
(N'Bùi Thanh Phúc',       'btpse180029@fpt.edu.vn', '2004-01-26'),
(N'Hoàng Gia Hưng',       'hghse180030@fpt.edu.vn', '2003-08-13'),

(N'Nguyễn Văn Tín',       'nvtse180031@fpt.edu.vn', '2004-11-01'),
(N'Phạm Nhật Minh',       'pnmse180032@fpt.edu.vn', '2003-07-07'),
(N'Trần Quốc Bảo',        'tqbse180033@fpt.edu.vn', '2004-09-15'),
(N'Đặng Minh Hiếu',       'dmhse180034@fpt.edu.vn', '2003-04-20'),
(N'Lê Thanh Sơn',         'ltsse180035@fpt.edu.vn', '2004-05-25'),

(N'Nguyễn Hoàng Long',    'nhlse180036@fpt.edu.vn', '2003-02-18'),
(N'Võ Đức Thành',         'vdtse180037@fpt.edu.vn', '2004-03-30'),
(N'Bùi Minh Đức',         'bmdse180038@fpt.edu.vn', '2003-06-12'),
(N'Phạm Quốc Huy',        'pqhse180039@fpt.edu.vn', '2004-10-09'),
(N'Trần Gia Bảo',         'tgbse180040@fpt.edu.vn', '2003-11-27'),

(N'Nguyễn Minh Tâm',      'nmtse180041@fpt.edu.vn', '2004-01-08'),
(N'Lê Quốc Trung',        'lqtse180042@fpt.edu.vn', '2003-09-03'),
(N'Đỗ Thanh Bình',        'dtbse180043@fpt.edu.vn', '2004-07-19'),
(N'Phan Hoàng Nam',       'phnse180044@fpt.edu.vn', '2003-05-22'),
(N'Trịnh Minh Khoa',      'tmkse180045@fpt.edu.vn', '2004-08-16'),

(N'Nguyễn Đức Anh',       'ndase180046@fpt.edu.vn', '2003-10-05'),
(N'Vũ Quang Huy',         'vqhse180047@fpt.edu.vn', '2004-06-01'),
(N'Lê Nhật Minh',         'lnmse180048@fpt.edu.vn', '2003-12-21'),
(N'Trần Hoàng Phúc',      'thpse180049@fpt.edu.vn', '2004-04-11'),
(N'Phạm Gia Khánh',       'pgkse180050@fpt.edu.vn', '2003-08-29');

INSERT INTO Course (CourseName, SemesterId, SubjectId)
VALUES

-- =========================
-- Spring 2024
-- =========================
('PRF192_SE1701', 1, 1),
('MAE101_SE1702', 1, 2),
('CEA201_SE1703', 1, 3),
('SSL101_SE1704', 1, 4),
('CSI104_SE1705', 1, 5),

-- =========================
-- Summer 2024
-- =========================
('NWC204_SE1801', 2, 6),
('SSG104_SE1802', 2, 7),
('PRO192_SE1803', 2, 8),
('MAD101_SE1804', 2, 9),
('OSG202_SE1805', 2, 10),

-- =========================
-- Fall 2024
-- =========================
('CSD201_SE1901', 3, 11),
('DBI202_SE1902', 3, 12),
('LAB211_SE1903', 3, 13),
('WED201_SE1904', 3, 14),
('JPD113_SE1905', 3, 15),

-- =========================
-- Spring 2025
-- =========================
('JPD123_SE2001', 4, 16),
('PRJ301_SE2002', 4, 17),
('SWE201_SE2003', 4, 18),
('MAS291_SE2004', 4, 19),
('IOT102_SE2005', 4, 20),

-- =========================
-- Summer 2025
-- =========================
('WDU203_SE2101', 5, 21),
('PRN212_SE2102', 5, 22),
('SWR302_SE2103', 5, 23),
('SWT301_SE2104', 5, 24),
('SWP391_SE2105', 5, 25),

-- =========================
-- Fall 2025
-- =========================
('OJT202_SE2201', 6, 26),
('ENW493_SE2202', 6, 27);

INSERT INTO Enrollment (StudentId, CourseId, EnrollDate, Status)
VALUES

-- =========================
-- Spring 2024 Courses
-- =========================
(1, 1, '2024-01-10', 'Completed'),
(2, 1, '2024-01-11', 'Completed'),
(3, 2, '2024-01-12', 'Completed'),
(4, 2, '2024-01-13', 'Completed'),
(5, 3, '2024-01-15', 'Completed'),

(6, 3, '2024-01-16', 'Completed'),
(7, 4, '2024-01-17', 'Completed'),
(8, 4, '2024-01-18', 'Dropped'),
(9, 5, '2024-01-19', 'Completed'),
(10, 5, '2024-01-20', 'Completed'),

-- =========================
-- Summer 2024 Courses
-- =========================
(11, 6, '2024-05-15', 'Completed'),
(12, 6, '2024-05-16', 'Completed'),
(13, 7, '2024-05-17', 'Completed'),
(14, 7, '2024-05-18', 'Completed'),
(15, 8, '2024-05-19', 'Completed'),

(16, 8, '2024-05-20', 'Dropped'),
(17, 9, '2024-05-21', 'Completed'),
(18, 9, '2024-05-22', 'Completed'),
(19, 10, '2024-05-23', 'Completed'),
(20, 10, '2024-05-24', 'Completed'),

-- =========================
-- Fall 2024 Courses
-- =========================
(21, 11, '2024-09-18', 'Completed'),
(22, 11, '2024-09-19', 'Completed'),
(23, 12, '2024-09-20', 'Completed'),
(24, 12, '2024-09-21', 'Completed'),
(25, 13, '2024-09-22', 'Completed'),

(26, 13, '2024-09-23', 'Completed'),
(27, 14, '2024-09-24', 'Completed'),
(28, 14, '2024-09-25', 'Dropped'),
(29, 15, '2024-09-26', 'Completed'),
(30, 15, '2024-09-27', 'Completed'),

-- =========================
-- Spring 2025 Courses
-- =========================
(31, 16, '2025-01-10', 'Completed'),
(32, 16, '2025-01-11', 'Completed'),
(33, 17, '2025-01-12', 'Completed'),
(34, 17, '2025-01-13', 'Completed'),
(35, 18, '2025-01-14', 'Completed'),

(36, 18, '2025-01-15', 'Completed'),
(37, 19, '2025-01-16', 'Completed'),
(38, 19, '2025-01-17', 'Active'),
(39, 20, '2025-01-18', 'Completed'),
(40, 20, '2025-01-19', 'Completed'),

-- =========================
-- Summer 2025 Courses
-- =========================
(41, 21, '2025-05-15', 'Active'),
(42, 21, '2025-05-16', 'Completed'),
(43, 22, '2025-05-17', 'Completed'),
(44, 22, '2025-05-18', 'Completed'),
(45, 23, '2025-05-19', 'Completed'),

(46, 23, '2025-05-20', 'Completed'),
(47, 24, '2025-05-21', 'Active'),
(48, 24, '2025-05-22', 'Completed'),
(49, 25, '2025-05-23', 'Completed'),
(50, 25, '2025-05-24', 'Completed');