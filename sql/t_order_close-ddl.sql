CREATE TABLE `t_order_close` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `order_calendar_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `closed_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8;