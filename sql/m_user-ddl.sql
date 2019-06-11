CREATE TABLE `m_user` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `login_id` text NOT NULL,
  `login_password` text NOT NULL,
  `role` text NOT NULL,
  `name` text NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8;