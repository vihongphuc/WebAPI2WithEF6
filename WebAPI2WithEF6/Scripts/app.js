﻿var ViewModel = function () {
    var self = this;
    self.books = ko.observableArray();
    self.error = ko.observable();

    var booksUri = '/api/books/';

    function ajaxHelper(uri, method, data) {
        self.error(''); //Clear error message
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null
        }).fail(function (jqXHR, textStatus, errorThrown) {
            self.error(errorThrown);
       });
    }

    function getAllBooks() {
        ajaxHelper(booksUri, 'GET').done(function (data) {
            self.books(data);
        });

    }

    //Fetch the initial data
    getAllBooks();


    //handling detail events
    self.detail = ko.observable();
    self.getBookDetail = function (item) {
        ajaxHelper(booksUri + item.Id, 'GET').done(function (data) {
            self.detail(data);
        });
    }


    //Add new item
    //self.authors = ko.observableArray();
    //self.newBook = {
    //    Author: ko.observable(),
    //    Genre: ko.observable(),
    //    Price: ko.observable(),
    //    Title: ko.observable(),
    //    Year:ko.observable()
    //}
    //var authorsUri = 'api/authors';

    //function getAuthors() {
    //    ajaxHelper(authorsUri,'GET').done(function (data){
    //        self.authors(data); 
    //    });
    //}
    
    
};

ko.applyBindings(new ViewModel());