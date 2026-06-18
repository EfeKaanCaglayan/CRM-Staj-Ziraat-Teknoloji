CREATE INDEX idx_request_customerid
    ON ZTCRM.ServiceRequest(CustomerId);

CREATE INDEX idx_request_status
    ON ZTCRM.ServiceRequest(CurrentStatus);